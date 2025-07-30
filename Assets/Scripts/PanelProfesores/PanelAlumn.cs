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

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnValidate()
    {
    }

    public void UpdateInfo()
    {
        timeGame.SetActive(true);
        name.text = $"{listExcel.name} {listExcel.lastName}";
        timeGame.GetComponentInChildren<TextMeshProUGUI>().text = $"{listExcel.gameTime}";
        for (int i = 0; i < listExcel.listProgressTema.Count; i++)
        {
            GameObject tem = temaProgress[i];
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    RectTransform child = tem.transform.GetChild(0).GetComponent<RectTransform>();
                    TextMeshProUGUI text = child.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    text.text = systemProgressLevel.levels[i].name;
                }
                string textLevel = TextVerifyTem(systemProgressLevel.levels[i]);
                if (textLevel == "Completed!!!")
                {
                    Transform childSlider = temaProgress[i].transform.GetChild(1);
                    Slider slider = childSlider.GetComponent<Slider>();
                    slider.value = 1;
                }
                if (textLevel == "Missing")
                {
                    Transform childSlider = temaProgress[i].transform.GetChild(1);
                    Slider slider = childSlider.GetComponent<Slider>();
                    slider.value = 0;
                }
                if (j == 2)
                {
                    Transform child = tem.transform.GetChild(j);
                    TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                    text.text = TextVerifyTem(systemProgressLevel.levels[i]);
                }
            }
        }
    }
    public string TextVerifyTem(ProgressLevel level)
    {
        if (systemProgressLevel.VerifyTemCompleted(level))
        {
            return "Completed!!!";
        }
        return "Missing";
    }
}
