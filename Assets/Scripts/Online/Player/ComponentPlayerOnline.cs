using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentPlayerOnline : MonoBehaviour
{
    public static ComponentPlayerOnline singleton
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

    public static ComponentPlayerOnline _singleton;
    public AnimationController AnimationController; public GameObject model;
    // Start is called before the first frame update
    private void Awake()
    {
        _singleton = this;
    }

    public void ModelIsActive(bool active)
    {
        model = AnimationController.avatar;
        model.SetActive(active);
    }
}
