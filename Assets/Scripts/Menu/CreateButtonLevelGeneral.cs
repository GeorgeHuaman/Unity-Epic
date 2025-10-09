using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateButtonLevelGeneral : MonoBehaviour
{
    public List<LevelDataGeneral> levelsData = new List<LevelDataGeneral>();
    public GameObject parent;
    public GameObject prefab;
    void Start()
    {
        foreach (LevelDataGeneral levelData in levelsData)
        {
            GameObject go = Instantiate(prefab, parent.transform);
            RectTransform child = go.GetComponentInChildren<RectTransform>();
            TMP_Text temaText = child.GetComponentInChildren<TMP_Text>();
            temaText.text = levelData.name;
            go.GetComponent<Button>().onClick.AddListener(() => {
                LoadScene(levelData.levelName);
            });
        }
    }

    void LoadScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

}
