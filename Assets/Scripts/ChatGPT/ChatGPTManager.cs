using OpenAI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Voice.Dictation;
using Meta.WitAi.TTS.Utilities;
using Meta.WitAi.TTS.Data;

public class ChatGPTManager : MonoBehaviour
{
    [TextArea(5, 20)] public string info;
    [TextArea(5, 20)] public string scene;
    public int maxResponseWordLimit = 15;
    public OnResponseEvent onResponse;
    private OpenAIApi openAI = new OpenAIApi();
    [System.Serializable] public class OnResponseEvent : UnityEvent<string> { }

    public AppDictationExperience voiceToText;
    public TTSSpeaker ttsSpeaker;
    public GameObject panelIA;

    [Header("Actions NPC")]
    public List<NPCAction> actions;

    [Header("Emotion Actions")]
    public List<EmotionAction> emotionActions;

    // System prompt fijo
    private ChatMessage _systemMessage;

    // Historial de chat
    private List<ChatMessage> _messages = new List<ChatMessage>();

    // Cola de fragmentos para TTS con su emoción
    private List<(string text, string emotion)> _fragmentQueue = new List<(string, string)>();
    private int _currentFragmentIndex = 0;

    void Awake()
    {
        _systemMessage = new ChatMessage
        {
            Role = "system",
            Content =
                "Actúa como una asistente y profesora pensada para ayudar a los niños en sus preguntas.\n" +
                "Tu estilo es casual, eficiente y educada, con un tono seguro, calmado y con humor sutil cuando sea apropiado.\n\n" +

                "Tu objetivo es responder al mensaje del jugador o continuar la conversación.\n" +
                "Eres consciente de que las respuestas serán convertidas a voz, así que evita saludos genéricos como 'usuario/a' o frases forzadas.\n" +
                "Usa un lenguaje claro, natural y fluido.\n\n" +

                "Responde de forma breve y concreta por defecto.\n" +
                "Si el jugador solicita una explicación más detallada, puedes explayarte, pero sin superar " + maxResponseWordLimit + " palabras.\n\n" +

                "Explica siempre los conceptos de modo que un niño pueda entenderlos: usa ejemplos sencillos, comparaciones fáciles y oraciones cortas.\n" +
                "Además, puedes ampliar cualquier punto más allá de la información dada, siempre que esté directamente relacionado con el tema.\n\n" +

                "Está permitido interactuar de forma casual y divertida si el jugador es un niño o si el contexto lo permite, pero siempre dentro de tu rol.\n" +
                "No respondas insultos, preguntas sin sentido o que no estén relacionadas al tema.\n" +
                "No inventes ni agregues información que no esté contenida en la sección de 'Tema', salvo que sea una ampliación razonable y relacionada.\n" +
                "No hables de ti mismo como IA salvo que te lo pidan directamente.\n" +
                "Nunca rompas personaje, salvo que explícitamente se te indique cambiar de rol.\n" +
                "Recuerda que si tienes que explicar algo, hazlo de forma que un niño pueda entenderlo fácil.\n\n" +

                "Aquí está la información del Tema:\n" +
                info + "\n\n" +

                "Aquí está la información sobre la escena que te rodea:\n" +
                scene + "\n\n" +

                buildActionInstruction() + "\n\n" +

                "Tras CADA ORACIÓN que generes, inserta inmediatamente un marcador de emoción entre corchetes, " +
                "Solo usa estos [EMOCIÓN: Feliz], [EMOCIÓN: Normal], [EMOCIÓN: Guiño], [EMOCIÓN: Asombrado], [EMOCIÓN: Sarcasmo], [EMOCIÓN: Apagado], [EMOCIÓN: Happy], [EMOCIÓN: Wink], [EMOCIÓN: Amazed], [EMOCIÓN: Sarcastic], [EMOCIÓN: Turnoff]. \n" +

                 "Quiero que, justo después de cada punto, coma o signo de exclamación, pongas algo como:\n" +
                "¡Muy bien! [EMOCIÓN: Feliz] ¿Listo para continuar? [EMOCIÓN: Asombrado]\n\n"
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            voiceToText.Activate();
    }

    public async void AskChatGPT(string newText)
    {
        //Agrega el mensaje del usuario al historial
        _messages.Add(new ChatMessage
        {
            Role = "user",
            Content = newText
        });

        // 2) Construye la lista de mensajes para la petición (system + últimos N)
        const int maxTurns = 8 * 2; // 8 intercambios usuario+IA
        var req = new List<ChatMessage> { _systemMessage };
        // Sólo los últimos maxTurns de _messages
        int start = Mathf.Max(0, _messages.Count - maxTurns);
        for (int i = start; i < _messages.Count; i++)
            req.Add(_messages[i]);

        // 3) Llamada a la API
        var response = await openAI.CreateChatCompletion(new CreateChatCompletionRequest
        {
            Model = "gpt-4.1-mini",
            Messages = req
        });

        if (response.Choices != null && response.Choices.Count > 0)
        {
            string raw = response.Choices[0].Message.Content;
            // 4) Extrae emociones con Regex
            var regex = new Regex(@"\[(?:EMOCIÓN|EMOCION|EMOTION):\s*(.*?)\]",
                                  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            Debug.Log(raw);
            // 5) Limpia texto plano
            string clean = regex.Replace(raw, "").Trim();

            // 6) Dispara tus acciones por keyword
            foreach (var act in actions)
                if (clean.Contains(act.actionKeyword))
                {
                    clean = clean.Replace(act.actionKeyword, "");
                    act.actionEvent.Invoke();
                }

            // 7) Guarda IA en historial
            _messages.Add(new ChatMessage
            {
                Role = "assistant",
                Content = clean
            });

            // 8) Callback de texto
            onResponse.Invoke(clean);

            // 9) Genera cola de (frase, emoción)
            _fragmentQueue.Clear();
            _currentFragmentIndex = 0;
            var fragments = Regex.Split(clean, @"(?<=[\.!?])\s+");
            string tempRaw = raw;
            foreach (var frag in fragments)
            {
                if (string.IsNullOrWhiteSpace(frag)) continue;
                var m = regex.Match(tempRaw);
                string emo = m.Success ? m.Groups[1].Value.Trim() : null;
                if (m.Success)
                    tempRaw = tempRaw.Substring(m.Index + m.Length);
                _fragmentQueue.Add((frag.Trim(), emo));
            }

            // 10) Detén audio previo
            ttsSpeaker.Stop();
            // ttsSpeaker.StopAllQueued(); // si existe

            // 11) Inicia reproducción secuencial
            PlayNextFragment();
        }
    }

    private void PlayNextFragment()
    {
        if (_currentFragmentIndex >= _fragmentQueue.Count) return;

        var (text, emotion) = _fragmentQueue[_currentFragmentIndex];
        // dispara emoción
        if (!string.IsNullOrEmpty(emotion))
            foreach (var emo in emotionActions)
                if (emo.emotionKeyword.Equals(emotion, System.StringComparison.OrdinalIgnoreCase))
                {
                    emo.emotionEvent.Invoke();
                    break;
                }

        // reproduce solo esa frase
        ttsSpeaker.Speak(text);
        _currentFragmentIndex++;
    }

    private void OnTTSPlaybackComplete(TTSSpeaker speaker, TTSClipData clipData)
    {
        // si no hubo error, avanza
        if (clipData.loadState != TTSClipLoadState.Error)
            PlayNextFragment();
    }

    public string buildActionInstruction()
    {
        string instr = "";
        foreach (var item in actions)
        {
            instr += $"Si quiero que hagas: {item.actionsDescription}, incluye la palabra clave “{item.actionKeyword}”.\n";
        }
        return instr;
    }

    public void ButtonChatGpt()
    {
        var gm = GameManager.Instance;
        gm.SetIsCanvasOpen(!panelIA.activeSelf);
        panelIA.SetActive(gm.IsCanvasOpen());
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
        public string emotionKeyword;   // “Feliz”, “Happy”, etc.
        public UnityEvent emotionEvent;
    }
}
