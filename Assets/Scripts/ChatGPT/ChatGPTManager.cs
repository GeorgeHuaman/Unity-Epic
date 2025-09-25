// using OpenAI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Voice.Dictation;
using System;
using System.Linq;
using WebSocketSharp;
using System.Collections;
using OpenAI.Images;
using OpenAI.Audio;
using OpenAI.Chat;
using System.IO;
using Unity.VisualScripting;


public class ChatGPTManager : MonoBehaviour
{
    [Header("Personalidad")]
    public PersonalityData personalidadActual;

    // [TextArea(5, 20)] public string info;
    // [TextArea(5, 20)] public string scene;
    // [TextArea(5, 20)] public string extraInstruction;

    // Instrucciones específicas para el adivina quien
    [TextArea(5, 20)] public string guessWhoInstructions;

    public int maxResponseWordLimit = 15;
    public bool useVoiceFriendly = false;
    public OnResponseEvent onResponse;
    [System.Serializable] public class OnResponseEvent : UnityEvent<string> { }

    public AppDictationExperience voiceToText;
    // public TTSSpeaker ttsSpeaker;
    public GameObject panelIA;

    [Header("Actions NPC")]
    public List<NPCAction> actions;

    [Header("Emotion Actions")]
    public List<EmotionAction> emotionActions;

    // private OpenAIApi openAI; // desinstalar una vez eliminado del flujo

    // System prompt fijo
    private ChatMessage systemMessage;

    // Historial de chat
    private List<ChatMessage> messages = new List<ChatMessage>();

    // Cola de fragmentos para TTS con su emoción
    private List<(string text, string emotion)> fragmentQueue = new List<(string, string)>();
    private int currentFragmentIndex = 0;
    public int uses = 0;
    public string modeloTexto = "gpt-4.1-mini";

    private AudioClient audioClient;
    private ChatClient chatClient;
    private ImageClient imageClient;



    /*
        Modelos usados actualmente:
        - texto: gpt-4.1-mini
        - imagenes: dall-e-3
        - voz / TTS: gpt-4o-mini-tts
    */


    string getApiKey()
    {
        var credAsset = Resources.Load<TextAsset>("auth");
        var auth = JsonUtility.FromJson<AuthData>(credAsset.text);
        return auth.api_key.Trim();
    }


    void Awake()
    {
        // Carga credenciales y modelos
        chatClient = new ChatClient(modeloTexto, getApiKey());
        imageClient = new ImageClient(modeloImagen, getApiKey());
        audioClient = new AudioClient(modeloVoz, getApiKey());
    }

    private void Start()
    {
        voiceToText.DictationEvents.OnFullTranscription.AddListener(AskChatGPT);
        // ttsSpeaker.Events.OnPlaybackComplete.AddListener(OnTTSPlaybackComplete);

    }

    private void OnDestroy()
    {
        voiceToText.DictationEvents.OnFullTranscription.RemoveListener(AskChatGPT);
        // ttsSpeaker.Events.OnPlaybackComplete.RemoveListener(OnTTSPlaybackComplete);
        chatClient = null;
        imageClient = null;
        audioClient = null;
    }

    public async void AskChatGPT(string newText)
    {

        // Revisamos si se pide una imagen mediante regex
        var imageMatch = Regex.Match(newText, @"\b(genera una imagen de|dibuja|haz un dibujo de|create an image of|draw)\b\s+(.+)", RegexOptions.IgnoreCase);
        if (imageMatch.Success)
        {
            string prompt = imageMatch.Groups[2].Value.Trim();
            GenerateImageFromDalle(prompt);
            return; // No sigas con el flujo de chat
        }

        DetectRoleplay(newText);

        systemMessage = new SystemChatMessage(BuildFullSystemPrompt());

        Debug.Log("mensaje inicial:" + systemMessage.Content[0].Text);

        messages.Add(new UserChatMessage(newText));

        var req = BuildRequestMessages();

        var response = await chatClient.CompleteChatAsync(req);

        if (response == null) return;

        string raw = response.Value.Content[0].Text;
        Debug.Log("mensaje raw" + raw);

        // Limpia los marcadores de emoción
        var emotionRegex = new Regex(@"\[(?:EMOCIÓN|EMOCION|EMOTION):\s*(.*?)\]", RegexOptions.IgnoreCase);
        if (!emotionRegex.IsMatch(raw))
        {
            var (textComplete, voiceComplete) = textHandler(raw);

            // 1. TTS con texto limpio
            await SpeakWithOpenAITTS(voiceComplete);

            // 2. Mostrar texto limpio en pantalla
            onResponse.Invoke(textComplete);
        }
        else
        {
            string textComplete = emotionRegex.Replace(raw, "").Trim();

            // 2. Mostrar texto limpio en pantalla
            onResponse.Invoke(textComplete);
            // 1. TTS con texto limpio

            await SpeakWithOpenAITTS(textComplete);
            foreach (Match match in emotionRegex.Matches(raw))
            {
                string emotion = match.Groups[1].Value.Trim();
                TriggerEmotion(emotion);
            }

        }

        // if (useVoiceFriendly)
        // {
        //     HandleVoiceFriendlyResponse(raw);
        // }
        // else
        // {
        //     HandleStandardResponse(raw);
        // }

        // Guardar la respuesta cruda para historial
        messages.Add(new AssistantChatMessage(raw));
        // AddUsesInExcell();
    }

    private List<ChatMessage> BuildRequestMessages()
    {
        // revisar si el MaxTurns puede aumentarse
        const int maxTurns = 16;
        var req = new List<ChatMessage> { systemMessage };
        int start = Mathf.Max(0, messages.Count - maxTurns);
        for (int i = start; i < messages.Count; i++)
            req.Add(messages[i]);

        // debug log de todo el historial
        // Debug.Log("Historial de mensajes:");
        // foreach (var msg in req)
        //     Debug.Log($"{msg.GetType().Name}: {msg.Content[0].Text}");

        return req;
    }

    private (string written, string voiceOnly) textHandler(string text)
    {
        // Primero revisemos regex de emocion
        var written = ExtractBetween(text, @"\*\*ESCRITA:\*\*\s*(.+?)(?=\r?\n\*\*VOZ:\*\*|\z)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        var voiceOnly = ExtractBetween(text, @"\*\*VOZ:\*\*\s*(.+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (string.IsNullOrWhiteSpace(written) && string.IsNullOrWhiteSpace(voiceOnly))
        {
            var parts = text.Split(new string[] { "\n\n" }, 2, System.StringSplitOptions.None);
            written = parts.Length > 0 ? parts[0].Trim() : text.Trim();
            voiceOnly = parts.Length > 1 ? parts[1].Trim() : written;
        }
        if (string.IsNullOrWhiteSpace(voiceOnly))
            voiceOnly = written;
        
        return (written, voiceOnly);
    }

    // private void HandleVoiceFriendlyResponse(string raw)
    // {
    //     // var written = ExtractBetween(raw, @"\*\*ESCRITA:\*\*(.+?)(?=\r?\n\*\*VOZ:\*\*|\z)");
    //     // var voiceOnly = ExtractBetween(raw, @"\*\*VOZ:\*\*(.+)\z");
    //     //Regex un poco mas tolerante con espacios y saltos de linea
    //     var written = ExtractBetween(raw, @"\*\*ESCRITA:\*\*\s*(.+?)(?=\r?\n\*\*VOZ:\*\*|\z)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
    //     var voiceOnly = ExtractBetween(raw, @"\*\*VOZ:\*\*\s*(.+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);


    //     if (string.IsNullOrWhiteSpace(written) && string.IsNullOrWhiteSpace(voiceOnly))
    //     {
    //         var parts = raw.Split(new string[] { "\n\n" }, 2, System.StringSplitOptions.None);
    //         written = parts.Length > 0 ? parts[0].Trim() : raw.Trim();
    //         voiceOnly = parts.Length > 1 ? parts[1].Trim() : written;
    //     }

    //     if (string.IsNullOrWhiteSpace(voiceOnly))
    //         voiceOnly = written;


    //     onResponse.Invoke(written);
    //     EnqueueVoice(voiceOnly);
    // }

    private void HandleStandardResponse(string emoraw)
    {
        var emotionRegex = new Regex(@"\[(?:EMOCIÓN|EMOCION|EMOTION):\s*(.*?)\]", RegexOptions.IgnoreCase);
        // Debug.Log("emoraw" + emoraw);

        string clean = emotionRegex.Replace(emoraw, "").Trim();

        TriggerActionsFromKeywords(clean);


        // Comentamos este add para que no se añanda doble al historial

        // messages.Add(new ChatMessage
        // {
        //     Role = "assistant",
        //     Content = clean
        // });

        onResponse.Invoke(clean);
        EnqueueFragmentsWithEmotions(clean, emoraw, emotionRegex);
        Voz.Stop();
        PlayNextFragment();
    }

    private void TriggerActionsFromKeywords(string text)
    {
        foreach (var act in actions)
        {
            if (text.Contains(act.actionKeyword))
            {
                text = text.Replace(act.actionKeyword, "");
                act.actionEvent.Invoke();
            }
        }
    }

    private void EnqueueFragmentsWithEmotions(string cleanText, string rawText, Regex emotionRegex)
    {
        fragmentQueue.Clear();
        currentFragmentIndex = 0;

        var fragments = Regex.Split(cleanText, @"(?<=[\.!?])\s+");
        string tempRaw = rawText;

        foreach (var frag in fragments)
        {
            if (string.IsNullOrWhiteSpace(frag)) continue;

            var match = emotionRegex.Match(tempRaw);
            string emotion = match.Success ? match.Groups[1].Value.Trim() : null;

            if (match.Success)
                tempRaw = tempRaw.Substring(match.Index + match.Length);

            fragmentQueue.Add((frag.Trim(), emotion));
        }
    }

    private void EnqueueVoice(string text)
    {
        Voz.Stop();

        text = Regex.Replace(text, @"\b[cC]\b", "ce");

        var emotionRegex = new Regex(@"\[(?:EMOCIÓN|EMOCION|EMOTION):\s*(.*?)\]", RegexOptions.IgnoreCase);

        foreach (Match match in emotionRegex.Matches(text))
        {
            string emotion = match.Groups[1].Value.Trim();
            TriggerEmotion(emotion);
        }

        string cleaned = emotionRegex.Replace(text, "").Trim();

        fragmentQueue.Clear();
        currentFragmentIndex = 0;

        var fragments = Regex.Split(cleaned, @"(?<=[\.!?])\s+");
        foreach (var frag in fragments)
            if (!string.IsNullOrWhiteSpace(frag))
                fragmentQueue.Add((frag.Trim(), null));

        PlayNextFragment();
    }

    private void TriggerEmotion(string key)
    {
        foreach (var emo in emotionActions)
            if (emo.emotionKeyword.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                emo.emotionEvent.Invoke();
                break;
            }
    }

    private void PlayNextFragment()
    {
        if (currentFragmentIndex >= fragmentQueue.Count) return;

        var (text, emotion) = fragmentQueue[currentFragmentIndex];

        if (!string.IsNullOrEmpty(emotion))
            TriggerEmotion(emotion);


        SpeakWithOpenAITTS(text);
        currentFragmentIndex++;
    }

    // Extraer texto entre patrones más robusto
    private string ExtractBetween(string input, string pattern, RegexOptions options = RegexOptions.Singleline)
    {
        var match = Regex.Match(input, pattern, options);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    // private void OnTTSPlaybackComplete(TTSSpeaker speaker, TTSClipData clipData)
    // {
    //     if (clipData.loadState != TTSClipLoadState.Error)
    //         PlayNextFragment();
    // }

    private void PlayAudioAndContinue(AudioClip clip)
    {
        if (Voz == null)
        {
            Debug.LogError("No se ha asignado el AudioSource 'Voz' en el Inspector.");
            return;
        }

        Voz.clip = clip;
        Voz.Play();
        StartCoroutine(WaitForAudioToEnd());
    }

    private IEnumerator WaitForAudioToEnd()
    {
        // Espera a que termine el audio
        while (Voz.isPlaying)
            yield return null;

        // Cuando termina, reproduce el siguiente fragmento si hay
        PlayNextFragment();
    }

    public string buildActionInstruction()
    {
        string instr = "";
        foreach (var i in actions)
            instr += $"Si quiero que hagas: {i.actionsDescription}, usa “{i.actionKeyword}”.\n";
        return instr;
    }

    public void ButtonChatGpt()
    {
        var gm = GameManager.Instance;
        gm.SetIsCanvasOpen(!panelIA.activeSelf);
        panelIA.SetActive(gm.IsCanvasOpen());
    }
    private string BuildFullSystemPrompt()
    {
        string isGuessWhoActive = "";
        string roleplayInstruction = "";
        if (isGuessWho)
        {
            isGuessWhoActive = "A partir de ahora estaremos jugando adivina quien, NO QUIERO QUE ME DIGAS QUE PERSONAJE ERES a menos que me rinda o que\n" +
            $"Lo adivine. Recuerda siempre hablarme en ingles. Tus respuestas posibles son 'yes', 'no' o 'I can't tell'. Recuerda que eres {CurrentRole}. \n" +
            "Recuerda que solo me puedes responder yes, no o i can't tell. YO no elijo ningun personaje, solo tu, asi funciona esta interaccion. \n" +
            " \n";
        }
        else
            if (!CurrentRole.IsNullOrEmpty())
            roleplayInstruction = $"A partir de ahora, responde y actúa como si fueras {CurrentRole}. Mantén el personaje en todo momento a menos \n" +
            "que te indique lo contrario. Trata de que a partir de ahora las conversaciones ronden a tu personaje o se relacionen a el. Por ejemplo \n" +
            "puedes preguntar 'que mas quieres saber de mi?' \n";

        if (!string.IsNullOrEmpty(CurrentRole) || isGuessWho)
        {
            return
            isGuessWhoActive + roleplayInstruction;
            // "Adicional a eso, recuerda que funcionas como " + personalidadActual.PromptPersonalidad + "\n\n" +
            // "La forma en la que regresas las respuestaas debe ser la siguiente: \n\n" + personalidadActual.entregaDeRespuesta + "\n\n" +
            // "La información del Tema es la siguiente:\n" + personalidadActual.informacion + "\n\n" +
            // "Si el jugador solicita una explicación más detallada, puedes explayarte, pero sin superar " + maxResponseWordLimit + " palabras.\n\n" +
            // "También quiero que respondas con estos valores de voz: " + personalidadActual.Voz + "\n\n" +
            // "Tono: " + personalidadActual.Tono + "\n\n" +
            // "Estilo de Entrega: " + personalidadActual.EstiloDeEntrega + "\n\n" +
            // "pronunciacion: " + personalidadActual.Pronunciacion + "\n\n" +
            // " Por último, también quiero que seas proactivo al responder y hacer preguntas para reforzar el conocimiento o el entendimiento de lo \n" +
            // "que te haya preguntado el usuario. Puedes proponer juegos como 1 pregunta y 4 posibles respuestas, etc.";
        }
        else
        {
            return
            "Tu nombre es " + personalidadActual.nombre + ". " +
            "funcionas como " + personalidadActual.PromptPersonalidad + "\n\n" +
            "La forma en la que regresas las respuestaas debe ser la siguiente: \n\n" + personalidadActual.entregaDeRespuesta + "\n\n" +
            "La información del Tema es la siguiente:\n" + personalidadActual.informacion + "\n\n" +
            "Si el jugador solicita una explicación más detallada, puedes explayarte, pero sin superar " + maxResponseWordLimit + " palabras.\n\n" +
            " Por último, también quiero que seas proactivo al responder y hacer preguntas para reforzar el conocimiento o el entendimiento de lo \n" +
            "que te haya preguntado el usuario. Puedes proponer juegos como 1 pregunta y 4 posibles respuestas, etc.";
        }
    }

    // public void AddUsesInExcell()
    // {
    //     int actualUse = int.Parse(UserSession.Instance.usosIA);
    //     uses++;
    //     int totalUses = actualUse + uses;
    //     int fila = UserSession.Instance.sheetRowNumber;
    //     string celda = "U" + fila;
    //     GoogleSheetsAPI.instance.WriteDataFor(celda, celda, totalUses);
    // }

    [System.Serializable]
    public struct NPCAction
    {
        public string actionKeyword;
        [TextArea(2, 5)] public string actionsDescription;
        public UnityEvent actionEvent;
    }

    [System.Serializable]
    public struct EmotionAction
    {
        public string emotionKeyword;
        public UnityEvent emotionEvent;
    }

    [System.Serializable] public class AuthData { public string api_key; public string organization; }

    // Capacidad de RolePlay
    private string CurrentRole = "";
    private bool isGuessWho = false;

    private void DetectRoleplay(string userMessage)
    {
        // usaremos la variable isGuessWho para saber si estamos jugando adivina quien, sin embargo si no hay valor la variable, que sea inaccesible
        var guessWho = Regex.Match(userMessage, @"\b(juguemos adivina quien|play guess who)\b", RegexOptions.IgnoreCase);
        if (guessWho.Success && !guessWhoInstructions.IsNullOrEmpty())
        {
            isGuessWho = true;
            GetGuessWhoCharacter();
        }
        // Veamos si dejamos de jugar
        if (isGuessWho)
        {
            var temp = Regex.Match(userMessage, @"\b(let's stop playing|I won|done|something else|practice)\b", RegexOptions.IgnoreCase);
            if (temp.Success)
            {
                isGuessWho = false;
                CurrentRole = "";
            }
        }

        var match = Regex.Match(userMessage, @"(?:a partir de ahora eres|ahora eres|actua como|from now on you are|now you are|act like)\s+([^\.\n]+)", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            CurrentRole = match.Groups[1].Value.Trim();
        }
        else
        {
            // Ahora para cancelar el roleplay
            var noRole = Regex.Match(userMessage, @"\b(vuelve a ser tú|Sé tú|deja de ser|be yourself again)\b", RegexOptions.IgnoreCase);
            if (noRole.Success)
            {
                CurrentRole = "";
            }
        }
    }

    // Pequeña llamada a la API para obtener un personaje en el adivina quien
    // Implementacion oficial
    private void GetGuessWhoCharacter()
    {
        var req = new List<ChatMessage>
        {
            new SystemChatMessage(guessWhoInstructions)
        };

        ChatCompletion completion = chatClient.CompleteChat(req);

        var response = completion;

        if (response.Content != null && response.Content.Count > 0)
        {
            string personaje = response.Content[0].Text.Trim();
            CurrentRole = personaje;
        }
    }


    /*
        Creacion de imagenes

        En el Centro matematicas hay un RawImage llamado "DalleCanvas", ahi es donde podré las imágenes generadas

    */
    [Header("Generación de imagenes")]
    public string modeloImagen = "dall-e-3";
    public UnityEngine.UI.RawImage DalleCanvas;

    public async void GenerateImageFromDalle(string prompt)
    {
        string completeImagePrompt = "quiero que hagas una imagen de esto: " + prompt +
        "\n\n Siempre cumple con las politicas y jamás generes contenido no permitido.";
        // " Jamas olvides tus parametros de informacion: " + info + " y " + scene + " y " + extraInstruction;

        // usar gpt-image-1 es inviable debido al costo y poca documentacion del mismo.
        // ImageGenerationOptions options = new()
        // {
        //     Quality = GeneratedImageQuality.High,
        //     Size = GeneratedImageSize.W1792xH1024,
        //     Style = GeneratedImageStyle.Vivid
        // };

        GeneratedImage image = await imageClient.GenerateImageAsync(completeImagePrompt);

        Debug.Log("Imagen generada: " + image);

        if (image != null)
        {
            Debug.Log("Imagen generada con éxito: " + image.ImageUri);
            string imageUrl = image.ImageUri.ToString();
            Debug.Log("URL de la imagen generada: " + imageUrl);
            StartCoroutine(LoadImageFromUrl(imageUrl));
        }
        else
        {
            Debug.LogError("No se generó ninguna imagen.");
        }
    }

    private IEnumerator LoadImageFromUrl(string url)
    {
        using (UnityEngine.Networking.UnityWebRequest uwr = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log("Imagen descargada con éxito.");
                Texture2D texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(uwr);
                DalleCanvas.texture = texture;
                DalleCanvas.color = Color.white; // Asegúrate de que la imagen sea visible
            }
            else
            {
                Debug.LogError("Error al descargar la imagen: " + uwr.error);
            }
        }
    }



    /*
        Text to Speech con OpenAI TTS

        Usamos el modelo gpt-4o-mini-tts
        Documentacion: https://platform.openai.com/docs/guides/speech-to-text/text
    */


    [Header("Voz de la IA")]
    /*
        Las posibles voces son:
        - alloy
        - ash
        - ballad
        - coral
        - echo
        - fable
        - nova
        - onyx
        - sage
        -shimmer
        Puedes probarlas en: https://www.openai.fm/
    */
    public string openAIVoice = "alloy"; // Puedes cambiar la voz predeterminada aquí
    public string modeloVoz = "gpt-4o-mini-tts";
    public AudioSource Voz;
    
    public async System.Threading.Tasks.Task SpeakWithOpenAITTS(string texto, string selectedVoice = null, string model = null)
    {
        string voice = string.IsNullOrEmpty(selectedVoice) ? openAIVoice : selectedVoice;
        if (string.IsNullOrEmpty(model))
            model = this.modeloVoz;
        await OpenAITTSRequest(texto, voice, model);
    }

    private async System.Threading.Tasks.Task OpenAITTSRequest(string texto, string voice, string model)
    {
        string textoFinal = "voz: " + personalidadActual.Voz + ". \n" +
        "tono: " + personalidadActual.Tono + ". \n" +
        "estilo de entrega: " + personalidadActual.EstiloDeEntrega + ". \n" +
        "pronunciacion: " + personalidadActual.Pronunciacion + ". \n";

    #pragma warning disable
    var options = new SpeechGenerationOptions
    {
        Instructions = textoFinal
    };
    #pragma warning restore

    Debug.Log("Instrucciones TTS: " + texto);
    BinaryData speech = await audioClient.GenerateSpeechAsync(
        texto,
        voice,
        options
    );

    string tempPath = Path.Combine(Application.persistentDataPath, "openai_tts.mp3");
    File.WriteAllBytes(tempPath, speech.ToArray());

    // Carga el archivo como AudioClip y reprodúcelo
    using (UnityEngine.Networking.UnityWebRequest uwr = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip("file://" + tempPath, AudioType.MPEG))
    {
        var asyncOp = uwr.SendWebRequest();
        while (!asyncOp.isDone)
            await System.Threading.Tasks.Task.Yield();

        if (uwr.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(uwr);
            if (Voz == null)
                Debug.LogError("No se ha asignado el AudioSource 'Voz' en el Inspector.");
            else
                PlayAudioAndContinue(clip);
        }
        else
        {
            Debug.LogError("Error al reproducir el audio: " + uwr.error);
        }
    }
}
}

