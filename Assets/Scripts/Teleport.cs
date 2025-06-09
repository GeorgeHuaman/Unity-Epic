using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject teleport;
    public GameObject buttons;
    public List<GameObject> cartelesWord = new List<GameObject>();
    bool acces= false;
    //19
    public void Tepe()
    {
    }

    public void AgreedTeleport(GameObject carteles)
    {
        acces = true;
        if (cartelesWord.Count == 0)
        {
            cartelesWord.Add(carteles);
        }
        else
        {
            for (int i = 0; i < cartelesWord.Count; i++)
            {
                if (cartelesWord[i].name == carteles.name)
                {
                    acces = false;
                }
            }
        }
        if (acces)
        {
           cartelesWord.Add (carteles);
            Debug.LogWarning(cartelesWord.Count);
        }
        if(cartelesWord.Count >= 20)
        {
            buttons.SetActive(true);
        }

    }
}
