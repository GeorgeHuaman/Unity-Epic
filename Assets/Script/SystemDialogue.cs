using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemDialogue : MonoBehaviour
{
    public GameObject panel;
    public Dialogue dialogue; //Ocultar cuando ya se pase el scriptable por codigo o interacion
    [Header("Localizacion GameObjects")]
    public Transmitter transmitter;
    public Receiver receiver;
    [Header("Objecto Prefab")]
    public GameObject buttons;
    [Header("Ajuste de texto")]
    public float speed = 0.6f;

    Coroutine currentCoroutine;
    public GameObject actualCamera;


    private void Update()
    {
    }
    public void Interactuable(Dialogue init)
    {
        Clean();
        panel.SetActive(true);
        for (int i = 0; i < init.questions.Count; i++)
        {
            int index = i;
            GameObject nw = Instantiate(buttons, transmitter.questions.transform);
            Button button = nw.GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = init.questions[index].response;
            button.onClick.AddListener(() =>
            {
                /*Cambio de fotos o imagen 
                transmitter.photo.texture = init.questions[index].photo.mainTexture;
                receiver.photo.texture = init.response[index].photo.mainTexture;*/
                Write(init.response[index].response);
                });
        }
        GameObject buttonExit = Instantiate(buttons, transmitter.questions.transform);
        Button Exit = buttonExit.GetComponent<Button>();
        Exit.GetComponentInChildren<TextMeshProUGUI>().text = "Salir";
        Exit.onClick.AddListener(() =>
        {
            panel.SetActive(false);
            actualCamera.SetActive(false);
        });

    }

    public void ActivateCamera(GameObject camera)
    {
        actualCamera = camera;
        actualCamera.SetActive(true);
    }
        

    private void Clean()
    {
        receiver.text.text = "";
        foreach (Transform child in transmitter.questions.transform)
        {
            Destroy(child.gameObject);
        }
    }
    #region Texto
    public void Write(string message)
    {
        // Si ya estaba escribiendo algo, lo detiene
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        receiver.text.text = "";

        foreach (char c in message)
        {
            receiver.text.text += c;
            yield return new WaitForSeconds(speed);
        }
    }
    #endregion
}
[Serializable]
public class Transmitter
{
    public RawImage photo;
    public GameObject questions;
}

[Serializable]
public class Receiver
{
    public RawImage photo;
    public TextMeshProUGUI text;
}
