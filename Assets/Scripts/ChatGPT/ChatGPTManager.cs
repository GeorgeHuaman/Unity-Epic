using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Voice.Dictation;
using Meta.WitAi.TTS.Utilities; // Para TTSSpeaker

public class ChatGPTManager : MonoBehaviour
{
    [TextArea(5, 20)] public string info;
    [TextArea(5, 20)] public string scene;
    public int maxResponseWordLimit = 15;
    public OnResponseEvent onResponse;
    [System.Serializable] public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    public AppDictationExperience voiceToText;
    public TTSSpeaker ttsSpeaker;

    public GameObject panelIA;

    [Header("Actions NPC")]
    public List<NPCAction> actions;

    [Header("Emotion Actions")]
    public List<EmotionAction> emotionActions;

    private void Start()
    {
        voiceToText.DictationEvents.OnFullTranscription.AddListener(AskChatGPT);
    }

    private void OnDestroy()
    {
        voiceToText.DictationEvents.OnFullTranscription.RemoveListener(AskChatGPT);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            voiceToText.Activate();
    }

    public string GetIntruction()
    {
        string instruction =
            "Actúa como una asistente y profesora pensada para ayudar a los niños en sus preguntas.\n" +
            "Tu estilo es casual, eficiente y educada, con un tono seguro, calmado y con humor sutil cuando sea apropiado.\n\n" +

            "Tu objetivo es responder al mensaje del jugador o continuar la conversación\n" +
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

            // Prompt para insertar marcadores after each sentence
            "Tras CADA ORACIÓN que generes, inserta inmediatamente un marcador de emoción entre corchetes, " +
            "por ejemplo [EMOCIÓN: Feliz], [EMOCIÓN: Enojado], [EMOCIÓN: Guiño], [EMOCIÓN: CejaArriba], etc. " +
            "Quiero que, justo después de cada punto, coma o signo de exclamación, pongas algo como:\n" +
            "   ¡Muy bien! [EMOCIÓN: Feliz] ¿Listo para continuar? [EMOCIÓN: Entusiasmado]\n\n" +

            "Aquí está el mensaje del jugador:\n";

        return instruction;
    }


    public async void AskChatGPT(string newText)
    {
        // 1) Envío del mensaje del jugador
        var newMessage = new ChatMessage
        {
            Role = "user",
            Content = GetIntruction() + newText
        };
        messages.Add(newMessage);

        // 2) Llamada a la API
        var request = new CreateChatCompletionRequest
        {
            Model = "gpt-4.1-mini",
            Messages = messages
        };
        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            string content = chatResponse.Content;

            // 3) Detecta marcadores ES/EN de emoción y dispara sus eventos
            var regex = new Regex(@"\[(?:EMOCIÓN|EMOCION|EMOTION):\s*(.*?)\]",
                                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            foreach (Match match in regex.Matches(content))
            {
                string emotion = match.Groups[1].Value.Trim();
                foreach (var emo in emotionActions)
                {
                    if (emo.emotionKeyword.Equals(emotion, System.StringComparison.OrdinalIgnoreCase))
                    {
                        emo.emotionEvent.Invoke();
                        break;
                    }
                }
            }
            Debug.Log(content);
            // 4) Limpia todos los marcadores antes de hablar
            content = regex.Replace(content, "").Trim();

            // 5) Procesa acciones NPC basadas en keywords
            foreach (var item in actions)
            {
                if (content.Contains(item.actionKeyword))
                {
                    content = content.Replace(item.actionKeyword, "");
                    item.actionEvent.Invoke();
                }
            }

            // 6) Guarda el mensaje en el historial
            chatResponse.Content = content;
            messages.Add(chatResponse);

            // 7) Opcional: callback de texto
            ttsSpeaker.Stop();
            onResponse.Invoke(content);

            // 8) Encola cada frase por separado para TTS
            var fragments = Regex.Split(content, @"(?<=[\.!?])\s+");
            foreach (var frag in fragments)
            {
                if (string.IsNullOrWhiteSpace(frag)) continue;
                ttsSpeaker.SpeakQueued(frag.Trim());
            }
        }
    }

    public string buildActionInstruction()
    {
        string instr = "";
        foreach (var item in actions)
        {
            instr += "Si quiero que hagas: " + item.actionsDescription +
                     ", incluye en tu respuesta la palabra clave: " + item.actionKeyword + ".\n";
        }
        return instr;
    }

    public void ButtonChatGpt()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.SetIsCanvasOpen(!panelIA.activeSelf);
        panelIA.SetActive(gameManager.IsCanvasOpen());
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
        public string emotionKeyword;   // e.g. "Feliz", "Enojado", "Guiño"
        public UnityEvent emotionEvent; // evento a invocar
    }
}

