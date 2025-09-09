using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerInteractuable : MonoBehaviour
{
    public static ManagerInteractuable singleton
    {
        get => _singleton;
        set
        {
            if (value == null)
                _singleton = null;
            else if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Destroy(value);
            }
        }
    }

    public static ManagerInteractuable _singleton;
    public List<Interactable> interactables = new List<Interactable>();
    public GameObject player;

    private void Awake()
    {
        singleton = this;
    }
    private void OnDestroy()
    {
        if (singleton == this)
            singleton = null;
    }

    public void DistribuidEmptyPlayer()
    {
        for (int i = 0; i < interactables.Count; i++)
        {
            interactables[i].player = player;
        }
    }
    public void DistribuidEmptyPlayerClean()
    {
        interactables.Clear();
    }
}
