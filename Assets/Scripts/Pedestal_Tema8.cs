using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GrabObject;

public class Pedestal_Tema8 : MonoBehaviour
{
    public Mission mision;
    public GameObject correct;
    public GameObject incorrect;
    public GameObject letter;
    public ObjectTypes pedestal;
    ConfirmPedestal confirm;
    public bool red = false;
    public bool yellow = false;
    public bool blue = false;
    public bool green = false;

    private void Start()
    {
        confirm = ConfirmPedestal.instance;
    }

    public void Correct()
    {
        incorrect.SetActive(false);
        correct.SetActive(true);
        letter.SetActive(true);
        gameObject.GetComponent<Pedestal_Tema8>().enabled = false;
        if (red)
        { 
            confirm.Red();
            mision.CompleteTask(4);
        }
        if (yellow) 
        { 
            confirm.Yellow();
            mision.CompleteTask(6);
        }
        if (blue) 
        { 
            confirm.Blue();
            mision.CompleteTask(3);
        }
        if (green) 
        {
            confirm.Green();
            mision.CompleteTask(5);
        }
    }

    public void Incorrect()
    {
        incorrect.SetActive(true);
    }
}
