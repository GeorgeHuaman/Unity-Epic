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
    [TextArea(5, 20)] public string extraInstruction;
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
    private ChatMessage _systemMessage;

    // Historial de chat
    private List<ChatMessage> _messages = new List<ChatMessage>();

    // Cola de fragmentos para TTS con su emoción
    private List<(string text, string emotion)> _fragmentQueue = new List<(string, string)>();
    private int _currentFragmentIndex = 0;

    void Awake()
    {
        // Carga credenciales
        var credAsset = Resources.Load<TextAsset>("auth");
        var auth = JsonUtility.FromJson<AuthData>(credAsset.text);
        openAI = new OpenAIApi(auth.api_key.Trim());

        _systemMessage = new ChatMessage
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            voiceToText.Activate();
    }

    public async void AskChatGPT(string newText)
    {
        // Reconstruye el prompt del sistema cada vez
        _systemMessage = new ChatMessage
        {
            Role = "system",
            Content = BuildFullSystemPrompt()
        };

        _messages.Add(new ChatMessage { Role = "user", Content = newText });

        const int maxTurns = 8 * 2;
        var req = new List<ChatMessage> { _systemMessage };
        int start = Mathf.Max(0, _messages.Count - maxTurns);
        for (int i = start; i < _messages.Count; i++)
            req.Add(_messages[i]);

        var response = await openAI.CreateChatCompletion(new CreateChatCompletionRequest
        {
            Model = "gpt-4.1-mini",
            Messages = req
        });
        if (response.Choices == null || response.Choices.Count == 0) return;

        string raw = response.Choices[0].Message.Content;
        Debug.Log(raw);

        if (useVoiceFriendly)
        {
            string written = "";
            string voiceOnly = "";

            var escMatch = Regex.Match(raw, @"\*\*ESCRITA:\*\*(.+?)(?=\r?\n\*\*VOZ:\*\*|\z)",
                                       RegexOptions.Singleline);
            var vozMatch = Regex.Match(raw, @"\*\*VOZ:\*\*(.+)\z",
                                       RegexOptions.Singleline);

            if (escMatch.Success && vozMatch.Success)
            {
                written = escMatch.Groups[1].Value.Trim();
                voiceOnly = vozMatch.Groups[1].Value.Trim();
            }
            else
            {
                var parts = raw.Split(new string[] { "\n\n" }, 2, System.StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    written = parts[0].Trim();
                    voiceOnly = parts[1].Trim();
                }
                else
                {
                    written = raw.Trim();
                    voiceOnly = "";
                }
            }
            if (string.IsNullOrWhiteSpace(voiceOnly))
            {
                voiceOnly = written;
            }

            //  UI: solo la parte escrita
            onResponse.Invoke(written);

            EnqueueVoice(voiceOnly);
        }
        else
        {
            string emoraw = response.Choices[0].Message.Content;
            // 4) Extrae emociones con Regex
            var regex = new Regex(@"\[(?:EMOCIÓN|EMOCION|EMOTION):\s*(.*?)\]",
                                  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            Debug.Log(emoraw);
            // 5) Limpia texto plano
            string clean = regex.Replace(emoraw, "").Trim();

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
            string tempRaw = emoraw;
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

        // 6) Guarda la respuesta (raw) para gestionar el historial
        _messages.Add(new ChatMessage { Role = "assistant", Content = raw });
    }

    // Divide un texto en frases, ignora emociones (ya disparadas) y encola
    private void EnqueueVoice(string text)
    {
        // Detiene audio anterior
        ttsSpeaker.Stop();

        // Extrae y dispara emociones inline si quedara alguna
        var emoRegex = new Regex(@"\[(?:EMOCIÓN|EMOCION|EMOTION):\s*(.*?)\]",
                                 RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        foreach (Match m in emoRegex.Matches(text))
        {
            string key = m.Groups[1].Value.Trim();
            foreach (var emo in emotionActions)
                if (emo.emotionKeyword.Equals(key, System.StringComparison.OrdinalIgnoreCase))
                {
                    emo.emotionEvent.Invoke();
                    break;
                }
        }
        // Limpia etiquetas
        text = emoRegex.Replace(text, "").Trim();

        // Prepara la cola de fragments
        _fragmentQueue.Clear();
        _currentFragmentIndex = 0;
        var frags = Regex.Split(text, @"(?<=[\.!?])\s+");
        foreach (var f in frags)
            if (!string.IsNullOrWhiteSpace(f))
                _fragmentQueue.Add((f.Trim(), null));

        // Empieza a hablar
        PlayNextFragment();
    }

    private void PlayNextFragment()
    {
        if (_currentFragmentIndex >= _fragmentQueue.Count) return;
        var (t, emo) = _fragmentQueue[_currentFragmentIndex];
        if (!string.IsNullOrEmpty(emo))
            foreach (var e in emotionActions)
                if (e.emotionKeyword.Equals(emo, System.StringComparison.OrdinalIgnoreCase))
                {
                    e.emotionEvent.Invoke();
                    break;
                }
        ttsSpeaker.Speak(t);
        _currentFragmentIndex++;
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
        return
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
            buildActionInstruction() + "\n\n";
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
}