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
        if (red) confirm.Red();
        if (yellow) confirm.Yellow();
        if (blue) confirm.Blue();
        if (green) confirm.Green();
    }

    public void Incorrect()
    {
        incorrect.SetActive(true);
    }
}
