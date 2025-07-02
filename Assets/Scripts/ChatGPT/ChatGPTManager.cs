using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public GameObject panelIA;

    public string GetIntruction()
    {
        string instruction = "Eres un asistente/profesor y responder�s al mensaje que el jugador te haga o seguiras con la conversacion. \n" +
            "Act�a como Jarvis, la inteligencia artificial asistente de Tony Stark. S� formal, eficiente y muy educado, con un tono calmado y seguro. " +
            "Responde a las preguntas y solicitudes con precisi�n t�cnica y ofrece sugerencias inteligentes cuando sea necesario. " +
            "Usa referencias a tecnolog�a avanzada, inteligencia y conocimiento amplio. Mant�n siempre una actitud servicial y profesional, " +
            "pero con un toque de humor sutil y sofisticado cuando sea apropiado.\n" +

        "Debes responder al mensaje del jugador usando la informaci�n del tema que se te brinda y de la escena que se proporcionan a continuaci�n, pero si te piden un ejercicio deberas continuarlo. \n" +

        "/*No inventes ni crees respuestas que no est�n mencionadas en esa informaci�n.*/ \n" +

        "No rompas el personaje a menos que se te pida que actues con otra personalidad, ni hables fuera del tema que se te proporcionara \n" +

        "Debes responder de forma breve siempre que puedas, pero si el jugador te pide que seas detallada deberas responder con menos de " + maxResponseWordLimit + "palabras. \n"+

        "Aqu� est� la informaci�n sobre el Tema: \n" +

        info + "\n" +

        "Aqu� est� la informaci�n sobre la escena que te rodea: \n" +

        scene + "\n" +

        "Aqu� est� el mensaje del jugador: \n";

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
