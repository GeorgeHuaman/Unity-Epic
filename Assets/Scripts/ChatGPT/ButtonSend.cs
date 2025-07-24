using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonSend : MonoBehaviour
{
    public ChatGPTManager chatGPTManager;
    public TMP_InputField inputField;

    [Tooltip("Tiempo mínimo (en segundos) entre clics consecutivos")]
    public float minClickInterval = 1.0f;

    private float lastClickTime = -Mathf.Infinity;

    public void SendMessage()
    {
        if (Time.time - lastClickTime < minClickInterval)
            return;

        lastClickTime = Time.time;

        chatGPTManager.AskChatGPT(inputField.text);
        inputField.text = string.Empty;
    }
}
