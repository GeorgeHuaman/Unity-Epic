using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class PanelAlumn : MonoBehaviour
{
    public ListExcel listExcel;
    public List<GameObject> temaProgress = new List<GameObject>();
    public TextMeshProUGUI name;
    public GameObject timeGame;
    public ProgressLevelSystem systemProgressLevel;
    // Start is called before the first frame update
    void Start()
    {
        systemProgressLevel = ProgressLevelSystem.instance;
    }
    public void UpdateInfo()
    {
        if (systemProgressLevel == null)
            systemProgressLevel = ProgressLevelSystem.instance;
        timeGame.SetActive(true);
        name.text = $"{listExcel.name} {listExcel.lastName}";
        timeGame.GetComponentInChildren<TextMeshProUGUI>().text = $"{listExcel.gameTime}";
        for (int i = 0; i < listExcel.listProgressTema.Count; i++)
        {
            GameObject tem = temaProgress[i];
            for (int j = 0; j < 2; j++)
            {
                if (j == 0)
                {
                    RectTransform child = tem.transform.GetChild(0).GetComponent<RectTransform>();
                    TextMeshProUGUI text = child.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    text.text = systemProgressLevel.levels[i].name;
                }
                Transform childSlider = temaProgress[i].transform.GetChild(1);
                Slider slider = childSlider.GetComponent<Slider>();
                slider.value = float.Parse(listExcel.listProgressTema[i]) / 100f;

                if (j == 1)
                {
                    Transform child = tem.transform.GetChild(j);
                    TextMeshProUGUI text = child.GetChild(3).GetComponent<TextMeshProUGUI>();
                    text.text = listExcel.listProgressTema[i] + "%";
                }
            }
        }
    }
}
