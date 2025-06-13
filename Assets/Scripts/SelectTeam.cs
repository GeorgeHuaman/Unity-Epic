using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTeam : MonoBehaviour
{
    public Sprite[] imageTeams;

    public Image canvasImage;

    public void SelectBlue()
    {
        canvasImage.gameObject.SetActive(true);
        canvasImage.sprite = imageTeams[0];
    }
    public void SelectRed()
    {
        canvasImage.gameObject.SetActive(true);
        canvasImage.sprite = imageTeams[1];
    }

}
