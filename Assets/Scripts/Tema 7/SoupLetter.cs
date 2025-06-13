using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SoupLetter : MonoBehaviour
{
    public TMP_InputField inputField;
    public string correct;
    public GameObject input;
    public Animator DoorGood;
    public Animator DoorBad;
    public GameObject correctGameobject;
    public GameObject badGameobject;

    public void InputFieldIntroduce()
    {
        string inputText = Regex.Replace(inputField.text, @"\s+", "").Trim(); // Elimina espacios internos y externos
        string correctText = Regex.Replace(correct, @"\s+", "").Trim(); // Hace lo mismo con el texto correcto
        Debug.LogError($"inputField: [{inputText}] length: {inputText.Length}");
        Debug.LogError($"correct: [{correctText}] length: {correctText.Length}");

        if (string.Equals(inputText, correctText, StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogError("A");
            DoorGood.SetBool("OpenBool", true);
            badGameobject.SetActive(false);
            correctGameobject.SetActive(false);
            correctGameobject.SetActive(true);
        }
        else
        {
            DoorBad.SetBool("OpenBool", true);
            badGameobject.SetActive(false);
            correctGameobject.SetActive(false);
            badGameobject.SetActive(true);
        }

        input.SetActive(false);
    }
    public void InputFieldSalir()
    {
        input.SetActive(false);
    }
    public void InputFieldAparecer()
    {
        input.SetActive(true);
    }
}
