using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [ReadOnly]public string interactableID;
    public float interactableRadius;
    public float interactableRadiusVisibility;
    [HideInInspector]private MeshRenderer interactableMeshRenderer;

    public GameObject dialogueTextGameObject;
    private bool startDialogue;
    [HideInInspector]public TextMeshProUGUI dialogueText;
    public List<LineDialogue> lineasDeDialogo;
    public AudioSource audioSource;

    [HideInInspector] public GameObject player;
    public float delayEntreLineas = 1f;
    private int indiceActual = 0;
    bool range;
    private float distance;
    public enum type
    {
        None,
        Type1,
        Type2,
        Type3,
        Type4,
    }
    public type iconType;

    [Header("Unity Events")]
    public UnityEvent unityEvent;

    [Header("Animator Event")]
    public UnityEvent animatorEvent;

    [Header("Quest Event")]
    public UnityEvent questEvent;

    // Start is called before the first frame update
    void Start()
    {
        interactableMeshRenderer = GetComponent<MeshRenderer>();
        dialogueText = dialogueTextGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (range)
            RangeDialogue();
    }
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            AssignID();
        }
        if (this.GetComponent<SphereCollider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
            gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            GetComponent<SphereCollider>().radius = interactableRadiusVisibility;
            GetComponent<SphereCollider>().isTrigger = true;
        }
    }

    public void RangeDialogue()
    {
         distance = Vector3.Distance(this.transform.position, player.transform.position);
        if (distance <= interactableRadius && Input.GetKeyDown(KeyCode.F) && !startDialogue)
        {
            Debug.DrawLine(transform.position,player.transform.position);
            StartCoroutine(ReproducirDialogo());
        }
    }

    IEnumerator ReproducirDialogo()
    {
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
        Debug.Log("Diálogo terminado");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.tag=="Player")
        {
          range = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.transform.tag == "Player" && distance > interactableRadiusVisibility   )
        {
           range =  false;
        }
    }
    private void AssignID()
    {
        // Solo si no tiene ya un ID
        if (!string.IsNullOrEmpty(interactableID)) return;

        // Obtener todos los objetos con este script
        Interactable[] all = FindObjectsOfType<Interactable>();
        HashSet<string> existingIDs = new HashSet<string>();
        foreach (var s in all)
        {
            if (!string.IsNullOrEmpty(s.interactableID))
                existingIDs.Add(s.interactableID);
        }

        // Buscar el primer ID disponible
        int id = 0;
        while (existingIDs.Contains(id.ToString("D2")))
        {
            id++;
        }

        interactableID = id.ToString("D2");
        Debug.Log($"Asignado ID {interactableID} a {gameObject.name}", gameObject);
    }
}

[Serializable]
public class LineDialogue
{
    public AudioClip clip;
    [TextArea]
    public string text;

    public LineDialogue (AudioClip clip, string text)
    {
        this.clip = clip;
        this.text = text;
    }   
}
