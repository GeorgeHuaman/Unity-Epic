using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [ReadOnly] public string interactableID;
    public float interactableRadius;
    public float interactableRadiusVisibility;
    [HideInInspector] public GameObject player;
    [HideInInspector] public bool inputButton;
    public Animator animator;
    public GameObject button;
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
    public UnityEvent onEnterEvent;
    public UnityEvent onInteractEvent;
    public UnityEvent onExitEvent;

    [Header("Quest Event")]
    public UnityEvent questEvent;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        AssignEvent();
    }

    void Update()
    {
        if (RangeVisibility())
        {
            if (button)
                button.SetActive(true);
            Vector3 lookDir = Camera.main.transform.position - transform.position;
            lookDir.y = 0f;
            if (button)
                button.transform.forward = -lookDir;
        }
        else if (button)
        {
            button.SetActive(false);
        }

        if (RangePlayer())
        {
            if (Input.GetKeyDown(KeyCode.F) || inputButton)
                EnterEvent();
        }
    }

    public void EnterEvent()
    {
        inputButton = false;
        StartCoroutine(HandleInteraction());
    }

    private IEnumerator HandleInteraction()
    {
        onEnterEvent?.Invoke();
        yield return null;
        onInteractEvent?.Invoke();
        yield return null;
        onExitEvent?.Invoke();
    }

    public void SetAnimatorBoolTrue(string parameterName)
    {
        Animator animator = this.animator;
        if (animator != null)
        {
            animator.SetBool(parameterName, true);
        }
    }

    public void SetAnimatorBoolFalse(string parameterName)
    {
        Animator animator = this.animator;
        if (animator != null)
        {
            animator.SetBool(parameterName, false);
        }
    }

    public bool RangePlayer()
    {
        Vector3 horizontalDistance = new Vector3(
            transform.position.x - player.transform.position.x,
            0,
            transform.position.z - player.transform.position.z
        );

        float distance = horizontalDistance.magnitude;
        return distance <= interactableRadius;
    }

    public bool RangeVisibility()
    {
        Vector3 horizontalDistance = new Vector3(
            transform.position.x - player.transform.position.x,
            0,
            transform.position.z - player.transform.position.z
        );

        float distance = horizontalDistance.magnitude;
        return distance <= interactableRadiusVisibility;
    }

    public void ButtonsInteractuable(bool interactable)
    {
        inputButton = interactable;
    }

    private void AssignID()
    {
        if (!string.IsNullOrEmpty(interactableID)) return;

        Interactable[] all = FindObjectsOfType<Interactable>();
        HashSet<string> existingIDs = new HashSet<string>();
        foreach (var s in all)
        {
            if (!string.IsNullOrEmpty(s.interactableID))
                existingIDs.Add(s.interactableID);
        }

        int id = 0;
        while (existingIDs.Contains(id.ToString("D2")))
        {
            id++;
        }

        interactableID = id.ToString("D2");
        Debug.Log($"Asignado ID {interactableID} a {gameObject.name}", gameObject);
    }

    void AssignEvent()
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            ImageInteractuable imageInteractuable = parent.GetComponent<ImageInteractuable>();
            if (imageInteractuable != null)
            {
                onInteractEvent.AddListener(imageInteractuable.OpenUI);
            }
        }
    }
}