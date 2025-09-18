using OpenAI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Voice.Dictation;
using Meta.WitAi.TTS.Utilities;
using Meta.WitAi.TTS.Data;
using System;
using System.Linq;
using WebSocketSharp;

public class ChatGPTManager : MonoBehaviour
{
    [TextArea(5, 20)] public string info;
    [TextArea(5, 20)] public string scene;
    [TextArea(5, 20)] public string extraInstruction;

    // Instrucciones específicas para el adivina quien
    [TextArea(5, 20)] public string guessWhoInstructions;

    public int maxResponseWordLimit = 15;
    public bool useVoiceFriendly = false;
    public OnResponseEvent onResponse;
    [System.Serializable] public class OnResponseEvent : UnityEvent<string> { }

    public AppDictationExperience voiceToText;
    public TTSSpeaker ttsSpeaker;
    public GameObject panelIA;

    [Header("Actions NPC")]
    public List<NPCAction> actions;

    [Header("Emotion Actions")]
    public List<EmotionAction> emotionActions;

    private OpenAIApi openAI;

    // System prompt fijo
    private ChatMessage systemMessage;

    // Historial de chat
    private List<ChatMessage> messages = new List<ChatMessage>();

    // Cola de fragmentos para TTS con su emoción
    private List<(string text, string emotion)> fragmentQueue = new List<(string, string)>();
    private int currentFragmentIndex = 0;

    public int uses = 0;


    void Awake()
    {
        // Carga credenciales
        var credAsset = Resources.Load<TextAsset>("auth");
        var auth = JsonUtility.FromJson<AuthData>(credAsset.text);
        openAI = new OpenAIApi(auth.api_key.Trim());

        systemMessage = new ChatMessage
        {
            Role = "system",
            Content =
                "Actúa como una asistente y profesora pensada para ayudar a los niños en sus preguntas.\n" +
                "Tu estilo es casual, eficiente y educada, con un tono seguro, calmado y con humor sutil cuando sea apropiado.\n\n" +
                "Tu objetivo es responder al mensaje del jugador o continuar la conversación.\n" +
                "Eres consciente de que las respuestas serán convertidas a voz, así que evita saludos genéricos como 'usuario/a'.\n" +
                "Usa un lenguaje claro, natural y fluido.\n\n" +
                "Responde de forma breve y concreta por defecto.\n" +
                "Si el jugador solicita una explicación más detallada, puedes explayarte, pero sin superar " + maxResponseWordLimit + " palabras.\n\n" +
                "Explica siempre los conceptos de modo que un niño pueda entenderlos: usa ejemplos sencillos y oraciones cortas.\n\n" +
                "Aquí está la información del Tema:\n" + info + "\n\n" +
                "Aquí está la información sobre la escena que te rodea:\n" + scene + "\n\n" +
                extraInstruction + "\n\n" +
                buildActionInstruction() + "\n\n"

        };
    }

    private void Start()
    {
        voiceToText.DictationEvents.OnFullTranscription.AddListener(AskChatGPT);
        ttsSpeaker.Events.OnPlaybackComplete.AddListener(OnTTSPlaybackComplete);

    }

    private void OnDestroy()
    {
        voiceToText.DictationEvents.OnFullTranscription.RemoveListener(AskChatGPT);
        ttsSpeaker.Events.OnPlaybackComplete.RemoveListener(OnTTSPlaybackComplete);
    }

    public async void AskChatGPT(string newText)
    {

        DetectRoleplay(newText);

        systemMessage = new ChatMessage
        {
            Role = "system",
            Content = BuildFullSystemPrompt()
        };

        Debug.Log(systemMessage.Content);

        messages.Add(new ChatMessage { Role = "user", Content = newText });

        var req = BuildRequestMessages();

        var response = await openAI.CreateChatCompletion(new CreateChatCompletionRequest
        {
            Model = "gpt-4.1-mini",
            Messages = req
        });

        if (response.Choices == null || response.Choices.Count == 0) return;

        string raw = response.Choices[0].Message.Content;
        // Debug.Log(raw);

        if (useVoiceFriendly)
        {
            HandleVoiceFriendlyResponse(raw);
        }
        else
        {
            HandleStandardResponse(raw);
        }

        // Guardar la respuesta cruda para historial
        messages.Add(new ChatMessage { Role = "assistant", Content = raw });
        AddUsesInExcell();
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
        //     Debug.Log($"{msg.Role}: {msg.Content}");
        
        return req;
    }

    private void HandleVoiceFriendlyResponse(string raw)
    {
        var written = ExtractBetween(raw, @"\*\*ESCRITA:\*\*(.+?)(?=\r?\n\*\*VOZ:\*\*|\z)");
        var voiceOnly = ExtractBetween(raw, @"\*\*VOZ:\*\*(.+)\z");

        if (string.IsNullOrWhiteSpace(written) && string.IsNullOrWhiteSpace(voiceOnly))
        {
            var parts = raw.Split(new string[] { "\n\n" }, 2, System.StringSplitOptions.None);
            written = parts.Length > 0 ? parts[0].Trim() : raw.Trim();
            voiceOnly = parts.Length > 1 ? parts[1].Trim() : written;
        }

        if (string.IsNullOrWhiteSpace(voiceOnly))
            voiceOnly = written;

        onResponse.Invoke(written);
        EnqueueVoice(voiceOnly);
    }

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
        ttsSpeaker.Stop();
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
        ttsSpeaker.Stop();

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


        ttsSpeaker.Speak(text);
        currentFragmentIndex++;
    }

    private string ExtractBetween(string input, string pattern)
    {
        var match = Regex.Match(input, pattern, RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private void OnTTSPlaybackComplete(TTSSpeaker speaker, TTSClipData clipData)
    {
        if (clipData.loadState != TTSClipLoadState.Error)
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
        string roleplayInstruction =  "";
        if (isGuessWho)
        {
            isGuessWhoActive = "A partir de ahora estaremos jugando adivina quien, NO QUIERO QUE ME DIGAS QUE PERSONAJE ERES a menos que me rinda o que\n" +
            $"Lo adivine. Recuerda siempre hablarme en ingles. Tus respuestas posibles son 'yes', 'no' o 'I can't tell'. Recuerda que eres {CurrentRole}. \n" +
            "Recuerda que solo me puedes responder yes, no o i can't tell. YO no elijo ningun personaje, solo tu, asi funciona esta interaccion. \n" +
            "Adicionalmente, ";
        }
        else
            if (!CurrentRole.IsNullOrEmpty())
                roleplayInstruction = $"A partir de ahora, responde y actúa como si fueras {CurrentRole}. Mantén el personaje en todo momento a menos \n" +
                "que te indique lo contrario.\n\n";

        return
            roleplayInstruction + isGuessWhoActive +
            "Actúa como una asistente y profesora pensada para ayudar a los niños en sus preguntas.\n" +
            "Tu estilo es casual, eficiente y educada, con un tono seguro, calmado y con humor sutil cuando sea apropiado.\n\n" +
            "Tu objetivo es responder al mensaje del jugador o continuar la conversación.\n" +
            "Eres consciente de que las respuestas serán convertidas a voz, así que evita saludos genéricos como 'usuario/a'.\n" +
            "Usa un lenguaje claro, natural y fluido.\n\n" +
            "Responde de forma breve y concreta por defecto.\n" +
            "Si el jugador solicita una explicación más detallada, puedes explayarte, pero sin superar " + maxResponseWordLimit + " palabras.\n\n" +
            "Explica siempre los conceptos de modo que un niño pueda entenderlos: usa ejemplos sencillos y oraciones cortas.\n\n" +
            "Aquí está la información del Tema:\n" + info + "\n\n" +
            "Aquí está la información sobre la escena que te rodea:\n" + scene + "\n\n" +
            extraInstruction + "\n\n" +
            buildActionInstruction() + "\n\n" +
            " Por último, también quiero que seas proactivo al responder y hacer preguntas para reforzar el conocimiento o el entendimiento de lo \n" + 
            "que te haya preguntado el usuario. Puedes proponer juegos como 1 pregunta y 4 posibles respuestas, etc.";
    }

    public void AddUsesInExcell()
    {
        int actualUse = int.Parse(UserSession.Instance.usosIA);
        uses++;
        int totalUses = actualUse + uses;
        int fila = UserSession.Instance.sheetRowNumber;
        string celda = "U" + fila;
        GoogleSheetsAPI.instance.WriteDataFor(celda, celda, totalUses);
    }

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
        Debug.Log("Detectando roleplay en: " + userMessage);
        Debug.Log(!guessWhoInstructions.IsNullOrEmpty());
        // usaremos la variable isGuessWho para saber si estamos jugando adivina quien, sin embargo si no hay valor la variable, que sea inaccesible
        var guessWho = Regex.Match(userMessage, @"\b(juguemos adivina quien|play guess who)\b", RegexOptions.IgnoreCase);
        if (guessWho.Success && !guessWhoInstructions.IsNullOrEmpty())
        {
            isGuessWho = true;
            GetGuessWhoCharacter();
        }
        Debug.Log("isGuessWho: " + isGuessWho);
        // Veamos si dejamos de jugar
        if (isGuessWho)
        {
            var temp = Regex.Match(userMessage, @"\b(let's stop playing|I won|done|something else|practice)\b", RegexOptions.IgnoreCase);
            if (temp.Success)
            {
                Debug.Log("Saliendo de adivina quien");
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
    private async void GetGuessWhoCharacter()
    {

        var req = new List<ChatMessage>
        {
            new ChatMessage { Role = "system", Content = guessWhoInstructions}
        };

        var response = await openAI.CreateChatCompletion(new CreateChatCompletionRequest
        {
            Model = "gpt-4.1-mini",
            Messages = req
        });

        Debug.Log(response);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            string personaje = response.Choices[0].Message.Content.Trim();
            CurrentRole = personaje;
            Debug.Log("Personaje para Adivina Quién: " + CurrentRole);
        }
    }
}