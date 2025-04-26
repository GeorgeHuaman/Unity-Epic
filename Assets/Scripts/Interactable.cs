using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [ReadOnly]public string interactableID;
    public float interactableRadius;
    public float interactableRadiusVisibility;
    [HideInInspector]private MeshRenderer interactableMeshRenderer;
    public GameObject dialogueTextGameObject;
    [HideInInspector]public TextMeshProUGUI dialogueText;
    [HideInInspector] public GameObject player;

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
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.tag=="Player")
        {
            Debug.Log("QQQ");
        }
    }

    //IEnumerator DialogueStart()
    //{
    //    while (true)
    //    {
    //        if()
    //    }
    //    return null;
    //}
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
