using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject panelOptions;
    public GameObject exit;

    private bool isCanvasOpen;
    public bool isCursorLocked = true;

    void Start()
    {
        Instance = this;

        if (SceneManager.GetActiveScene().name != "Plaza Central")
        {
            exit.SetActive(true);
        }
        else
        {
            exit.SetActive(false);
        }
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }
    private void Update()
    {
        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;

        if (isCanvasOpen)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangerCursorMode();
        }
    }
    public bool IsCanvasOpen()
    {
        return isCanvasOpen;
    }
    public bool GetBoolCursorLocked()
    {
        return isCursorLocked;
    }

    public void SetIsCanvasOpen(bool set)
    {
        isCanvasOpen = set;
        SetCursorMode(!set);
    }

    public void ButtonOptions()
    {
        SetIsCanvasOpen(!panelOptions.activeSelf);
        panelOptions.SetActive(IsCanvasOpen());
    }
    public void ChangerCursorMode()
    {
        isCursorLocked = !isCursorLocked;
    }
    public void SetCursorMode(bool set)
    {
        isCursorLocked = set;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
