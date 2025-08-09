using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CreateButtonAlumn : MonoBehaviour
{
    public DataBaseAlumn dataBaseAlumn;
    public GameObject parent;
    public GameObject prefab;
    public GameObject textPanelAlumn;
    public PanelAlumn panelAlumn;
    public GameObject father;
    public static CreateButtonAlumn Instance;
    // Start is called before the first frame update

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        dataBaseAlumn = DataBaseAlumn.Instance;
    }

    public void CreateButton()
    {
        dataBaseAlumn = DataBaseAlumn.Instance;
        for (int i = 0; i < dataBaseAlumn.ExcelList.Count; i++)
        {
            int j = i;
            ListExcel excel = dataBaseAlumn.ExcelList[i];
            GameObject go = Instantiate(prefab, parent.transform);

            TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"{excel.name} {excel.lastName}";
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                SeePanelAlumn(j);
                panelAlumn.gameObject.SetActive(true);
                textPanelAlumn.gameObject.SetActive(true);
                father.gameObject.SetActive(false);
            });
        }
    }


    public void SeePanelAlumn(int i)
    {
        panelAlumn.listExcel = dataBaseAlumn.ExcelList[i];
        panelAlumn.UpdateInfo();
    }
}
