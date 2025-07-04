using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Return : MonoBehaviour
{
    public Vector3 init;
    public ObjectTypes types;
    public GrabObject grabObject;
    void Start()
    {
        init = transform.position;
        grabObject = GrabObject.instance;
        types.Object = this.gameObject;
    }
    public void Returned()
    {
        transform.position = init;
    }

    public void Data()
    {
        grabObject.PickObject(types);
        grabObject.types = types;
    }
}
