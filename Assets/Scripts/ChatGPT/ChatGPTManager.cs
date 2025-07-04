using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Oculus.Voice.Dictation;
public class ChatGPTManager : MonoBehaviour
{
    [TextArea(5,20)]
    public string info;
    [TextArea(5,20)]
    public string scene;
    public int maxResponseWordLimit = 15;

    public OnResponseEvent onResponse;

    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    public AppDictationExperience voiceToText;

    public GameObject panelIA;

    private void Start()
    {
        voiceToText.DictationEvents.OnFullTranscription.AddListener(AskChatGPT);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            voiceToText.Activate();
        }
    }
    public string GetIntruction()
    {
        string instruction =

            "Actúa como una asistente y profesora pensada para ayudar a los niños en sus preguntas.\n" +
            "Tu estilo es casual, eficiente y educada, con un tono seguro, calmado y con humor sutil cuando sea apropiado.\n\n" +

            "Tu objetivo es responder al mensaje del jugador o continuar la conversación, siempre usando la información que se te proporciona.\n" +
            "Eres consciente de que las respuestas serán convertidas a voz, así que evita saludos genéricos como 'usuario/a' o frases forzadas. Usa un lenguaje claro, natural y fluido.\n" +

            "Responde de forma breve y concreta por defecto.\n" +
            "Si el jugador solicita una explicación más detallada, puedes explayarte, pero sin superar " + maxResponseWordLimit + " palabras.\n\n" +

            "Explica siempre los conceptos de modo que un niño pueda entenderlos: usa ejemplos sencillos, comparaciones fáciles y oraciones cortas.\n" +
            "Además, puedes ampliar cualquier punto más allá de la información dada, siempre que esté directamente relacionado con el tema.\n\n" +

            "Está permitido interactuar de forma casual y divertida si el jugador es un niño o si el contexto lo permite, pero siempre dentro de tu rol.\n" +
            "No respondas insultos, preguntas sin sentido o que no estén relacionadas al tema.\n" +
            "No inventes ni agregues información que no esté contenida en la sección de 'Tema', salvo que sea una ampliación razonable y relacionada.\n" +
            "No hables de ti mismo como IA salvo que te lo pidan directamente.\n" +
            "Nunca rompas personaje, salvo que explícitamente se te indique cambiar de rol.\n" +
            "Recuerda que si tienes que explicar algo, hazlo de forma que un niño pueda entenderlo facil.\n"+

            "Aquí está la información del Tema:\n" +
            info + "\n\n" +

            "Aquí está la información sobre la escena que te rodea:\n" +
            scene + "\n\n" +

            "Aquí está el mensaje del jugador:\n";

        return instruction;
    }

    public async void AskChatGPT(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = GetIntruction() + newText;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var respone = await openAI.CreateChatCompletion(request);

        if (respone.Choices != null && respone.Choices.Count > 0)
        {
            var chatResponse = respone.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);

            onResponse.Invoke(chatResponse.Content);
        }
    }

    public void ButtonChatGpt()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.SetIsCanvasOpen(!panelIA.activeSelf);
        panelIA.SetActive(gameManager.IsCanvasOpen());
        
    }
}
