using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public Interactable interactable;
    public GameObject dialogueTextGameObject;
    private bool startDialogue;
    [HideInInspector] public TextMeshProUGUI dialogueText;
    public List<LineDialogue> lineasDeDialogo;
    public AudioSource audioSource;
    [HideInInspector] public GameObject player;
    public float delayEntreLineas = 1f;
    private int indiceActual = 0;
    // Start is called before the first frame update
    void Start()
    {
        dialogueText = dialogueTextGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable.RangePlayer() && !startDialogue && Input.GetKeyDown(KeyCode.F))
            RangeDialogue();
    }
    public void RangeDialogue()
    {
        StartCoroutine(ReproducirDialogo());
    }
    IEnumerator ReproducirDialogo()
    {
        dialogueText.gameObject.SetActive(true);
        startDialogue = true;
        while (indiceActual < lineasDeDialogo.Count)
        {
            LineDialogue linea = lineasDeDialogo[indiceActual];

            dialogueText.text = linea.text;
            audioSource.clip = linea.clip;
            audioSource.Play();

            yield return new WaitForSeconds(linea.clip.length + delayEntreLineas);

            indiceActual++;
        }
        indiceActual = 0;
        dialogueText.text = "";
        startDialogue = false;
        dialogueText.gameObject.SetActive(false);
        Debug.Log("Diálogo terminado");
    }
}
