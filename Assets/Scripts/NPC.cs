using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [HideInInspector]public bool startDialogue;
    public List<LineDialogue> lineasDeDialogo;
    public AudioSource audioSource;

    [HideInInspector] public GameObject player;
    public float delayEntreLineas = 1f;
    private int indiceActual = 0;
    public ControllerAudio controller;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Audio()
    {
        if (!startDialogue)
        {
           controller.StopAudios();
           controller.coroutine = StartCoroutine(ReproducirDialogo());
        }
    }
    IEnumerator ReproducirDialogo()
    {
        startDialogue = true;
        while (indiceActual < lineasDeDialogo.Count)
        {
            LineDialogue linea = lineasDeDialogo[indiceActual];

            audioSource.clip = linea.clip;
            audioSource.Play();

            yield return new WaitForSeconds(linea.clip.length + delayEntreLineas);

            indiceActual++;
        }
        indiceActual = 0;
        startDialogue = false;
        Debug.Log("Diálogo terminado");
    }
}

[Serializable]
public class LineDialogue
{
    public AudioClip clip;
    [TextArea]
    public string text;

    public LineDialogue(AudioClip clip, string text)
    {
        this.clip = clip;
        this.text = text;
    }
}