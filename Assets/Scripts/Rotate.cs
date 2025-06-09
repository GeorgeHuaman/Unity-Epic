using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public bool z = false;

    public bool y = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(z)
        transform.Rotate(0, 0, +1f);
        if(y)
            transform.Rotate(0, +1f,0f);
    }
}
