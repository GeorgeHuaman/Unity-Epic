using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject panelInicio;
    public GameObject panelLevels;
    public void Play()
    {
        SceneManager.LoadScene("Tema1 Kids");
    }
    public void Levels()
    {
        panelInicio.SetActive(false);
        panelLevels.SetActive(true);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
