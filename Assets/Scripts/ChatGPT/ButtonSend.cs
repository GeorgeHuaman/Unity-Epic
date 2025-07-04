using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonSend : MonoBehaviour
{
    public ChatGPTManager chatGPTManager;
    public TMP_InputField inputField;

    public void SendMessage()
    {
        chatGPTManager.AskChatGPT(inputField.text);
        inputField.text = string.Empty;
    }
}
