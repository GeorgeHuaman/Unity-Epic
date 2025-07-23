using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateButtonAlumn : MonoBehaviour
{
    public DataBaseAlumn dataBaseAlumn;
    public GameObject parent;
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < dataBaseAlumn.ExcelList.Count; i++)
        {
            ListExcel excel = dataBaseAlumn.ExcelList[i];
            GameObject go = Instantiate(prefab, parent.transform);

            TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"{excel.name} {excel.lastName}";
            GetComponent<Button>().onClick.AddListener(() =>
            {

            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
