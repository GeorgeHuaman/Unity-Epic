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
    [HideInInspector]public GameObject player;
    [HideInInspector]public bool range;
    [HideInInspector]public bool inputButton;
    public GameObject button;
    public GameObject icons;
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
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (range)
        {
            Vector3 lookDir = Camera.main.transform.position - transform.position;
            lookDir.y = 0f; // Solo gira en el plano XZ
            icons.transform.forward = lookDir;
            
        }

        if(RangePlayer())
        {
            button.SetActive(true);
            if(Input.GetKeyDown(KeyCode.F) || inputButton)
                 EnterEvent();
        }
    }

    private void EnterEvent()
    {
        inputButton = false;
        Debug.Log("FUnciona");
        //unityEvent.Invoke();
        //animatorEvent.Invoke();
        //questEvent.Invoke();
    }

    public bool RangePlayer()
    {
        distance = Vector3.Distance(this.transform.position, player.transform.position);
        if (distance <= interactableRadius)
            return true;
        return false;
    }

    public void ButtonsInteractuable(bool interactable)
    {
        inputButton = interactable;
    }

    #region Collider
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
    #endregion
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
