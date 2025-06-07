using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    public Transform hand;
    public bool handVerify = false;
    public static GrabObject instance;
    [HideInInspector] public ObjectTypes types;
    // Start is called before the first frame update
    void Start()
    {
        instance = this.GetComponent<GrabObject>();
    }

    public void PickObject(ObjectTypes types)
    {
        if (handVerify)
        {
            types.Object.transform.SetParent(null, false);
            types.Object.GetComponent<Return>().Returned();
        }
        types.Object.transform.SetParent(hand);
    }

    void Drop(ObjectTypes types)
    {
        handVerify = false;
        types.Object.transform.SetParent(null, false);
        types.Object.GetComponent<Return>().Returned();
    }

    public void VerifyPedestal(GameObject pedestal)
    {
        Pedestal_Tema8 pedest = pedestal.GetComponent<Pedestal_Tema8>();
        if(pedest.pedestal.objectType == types.objectType)
        {
            pedest.Correct();
            types.Object.transform.SetParent(null, false);
            types.Object.gameObject.SetActive(false);
            types = null;

        }
        else
        {
            pedest.Incorrect();
            Drop(types);
        }
        
    }

    [Serializable]
    public class ObjectTypes
    {
        public GameObject Object;
        public enum type
        {
            None,
            Red,
            Blue,
            Yellow,
            Green
        }
        public type objectType;

        public ObjectTypes(GameObject @object, type objectType)
        {
            Object = @object;
            this.objectType = objectType;
        }
    }
}