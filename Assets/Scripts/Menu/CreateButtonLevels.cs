using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CreateButtonLevels : MonoBehaviour
{
    public List<LevelData> levelsData = new List<LevelData>();
    public GameObject parent;
    public GameObject prefab;
    public SystemProgressLevel systemProgressLevel;
    private void Awake()
    {
        systemProgressLevel = FindAnyObjectByType<SystemProgressLevel>();
    }
    void Start()
    {
        foreach (LevelData levelData in levelsData)
        {
            GameObject go = Instantiate(prefab, parent.transform);
            for (int i = 0; i <= 4; i++)
            {
                if (i == 0)
                {
                    RectTransform child = go.GetComponentInChildren<RectTransform>();
                    TMP_Text temaText = child.GetComponentInChildren<TMP_Text>();
                    temaText.text = levelData.name;
                }
                else
                {
                    int index = i; // Necesario para capturar correctamente el índice en la lambda
                    Transform child = go.transform.GetChild(index);
                    child.GetComponent<Button>().onClick.AddListener(() => {
                        LoadScene(levelData.versions[index - 1].versionName); // -1 porque el primer hijo (i==0) era el título

                    });
                    child.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        SendInfoLevel(levelData.versions[index - 1].versionName, levelData);
                    });
                }
            }
        }
    }

    void LoadScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    void SendInfoLevel(string nameScene,LevelData version)
    {
        systemProgressLevel.IdentifyLevel(nameScene,version);
    }
}
