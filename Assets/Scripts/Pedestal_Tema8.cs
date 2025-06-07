using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GrabObject;

public class Pedestal_Tema8 : MonoBehaviour
{
    public GameObject correct;
    public GameObject incorrect;
    public GameObject letter;
    public ObjectTypes pedestal; 

    public void Correct()
    {
        incorrect.SetActive(false);
        correct.SetActive(true);
        letter.SetActive(true);
        gameObject.GetComponent<Pedestal_Tema8>().enabled = false;
    }

    public void Incorrect()
    {
        incorrect.SetActive(true);
    }
}
