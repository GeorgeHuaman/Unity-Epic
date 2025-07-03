using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfazProgressLevel : MonoBehaviour
{
    public SystemProgressLevel systemProgressLevel;
    public GameObject prefabTema;
    public GameObject parent;
    public GameObject panelProgress;
    public List<GameObject> levelProgress = new List<GameObject>();
    // Start is called before the first frame update

    private void Awake()
    {
        systemProgressLevel = FindAnyObjectByType<SystemProgressLevel>();
    }
    void Start()
    {
        InstanceEmpty();
    }

    public void InstanceEmpty()
    {
        Debug.Log(systemProgressLevel.levels.Count.ToString());
        for (int i = 0; i < systemProgressLevel.levels.Count; i++)
        {
            GameObject tem = Instantiate(prefabTema,parent.transform);
            levelProgress.Add(tem);
            for (int j = 0; j < 3; j++)
            {
                if(j == 0)
                {
                    RectTransform child = tem.transform.GetChild(0).GetComponent<RectTransform>();
                    TextMeshProUGUI text = child.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    text.text = systemProgressLevel.levels[i].name;
                }
                if (TextVerifyTem(systemProgressLevel.levels[i]) == "Completed!!!")
                {
                    Transform childSlider = levelProgress[i].transform.GetChild(1);
                    Slider slider = childSlider.GetComponent<Slider>();
                    slider.value = 1;
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
        if(systemProgressLevel.VerifyTemCompleted(level))
        {
            return "Completed!!!";
        }
        return "Missing";
    }
    public void PanelOpenOff()
    {
        panelProgress.SetActive(!panelProgress.activeSelf);
        if(panelProgress)
        {
            UpdateInfo();
        }
    }
    public void UpdateInfo()
    {
        if (levelProgress != null)
        {
            for (int i = 0; i < levelProgress.Count; i++)
            {
                if (TextVerifyTem(systemProgressLevel.levels[i]) == "Completed!!!")
                {
                    Transform childSlider = levelProgress[i].transform.GetChild(1);
                    Slider slider = childSlider.GetComponent<Slider>();
                    slider.value = 1;
                }
                for (int j = 0; j < 3; j++)
                {
                    if (j == 2)
                    {
                        Transform child = levelProgress[i].transform.GetChild(j);
                        TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                        text.text = TextVerifyTem(systemProgressLevel.levels[i]);
                    }
                   
                }
            }
        }
    }
}
