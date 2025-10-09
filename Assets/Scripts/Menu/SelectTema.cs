using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectTema : MonoBehaviour
{
    public List<GameObject> temaList = new List<GameObject>();
    public GameObject parent;
    public GameObject prefab;
    public GameObject panelGeneral;
    void Start()
    {
        foreach (GameObject levelData in temaList)
        {
            GameObject go = Instantiate(prefab, parent.transform);
            RectTransform child = go.GetComponentInChildren<RectTransform>();
            TMP_Text temaText = child.GetComponentInChildren<TMP_Text>();
            temaText.text = levelData.name;
            go.GetComponent<Button>().onClick.AddListener(() => { 
                levelData.SetActive(true);
                panelGeneral.SetActive(false);
            });
        }
    }
}
