using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SistemaNervioso : MonoBehaviour
{
    public GameObject[] esferas;
    public Button boton;
    public Mission quest;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Button()
    {
        StartCoroutine(Luces());
    }

    IEnumerator Luces()
    {
        boton.interactable = false;
        for (int i = 0; i < esferas.Length; i++)
        {
            esferas[i].SetActive(false);
        }
            
        yield return new WaitForSeconds(0.5f);
        {
            for (int i = 0; i < esferas.Length; i++)
            {
                esferas[i].SetActive(true);
            }
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < esferas.Length; i++)
        {
            esferas[i].SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        {
            for (int i = 0; i < esferas.Length; i++)
            {
                esferas[i].SetActive(true);
            }
        }
        quest.tasks[3].CompleteTask();
        boton.interactable = true;
    }
}
