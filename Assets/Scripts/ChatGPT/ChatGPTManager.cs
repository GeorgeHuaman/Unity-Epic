using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ChatGPTManager : MonoBehaviour
{
    [TextArea(5,20)]
    public string personality;
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
        string instruction = "Eres un asistente/profesora en un videojuego y responderás al mensaje que el jugador te haga. \n" +

        "Debes responder al mensaje del jugador únicamente usando la información de tu personalidad y de la escena que se proporcionan a continuación. \n" +

        "No inventes ni crees respuestas que no estén mencionadas en esa información. \n" +

        "No rompas el personaje \n" +

        "Debes responder en menos de " + maxResponseWordLimit + " palabras. \n" +

        "Aquí está la información sobre tu personalidad: \n" +

        personality + "\n" +

        "Aquí está la información sobre la escena que te rodea: \n" +

        scene + "\n" +

        "Aquí está el mensaje del jugador: \n";

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
