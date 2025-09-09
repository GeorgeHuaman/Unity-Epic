using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    private void Start()
    {
        LoaderAvatar.singleton.gameObject.transform.position = this.transform.position;
    }
}
