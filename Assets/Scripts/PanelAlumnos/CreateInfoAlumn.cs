using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateInfoAlumn : MonoBehaviour
{
    private UserSession userSession;
    public TextMeshProUGUI name;
    public GameObject timeGame;
    public ListExcel listExcel;
    public List<GameObject> temaProgress = new List<GameObject>();
    private ProgressLevelSystem systemProgressLevel;
    public static CreateInfoAlumn Instance;
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
    void Start()
    {
        userSession = UserSession.Instance;
        systemProgressLevel = ProgressLevelSystem.instance;
    }

    public void UpdateInfo()
    {
        if(systemProgressLevel == null)
            systemProgressLevel = ProgressLevelSystem.instance;

        listExcel = UserSession.Instance.InfoAlumn();
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
                Transform childSlider = temaProgress[i].transform.GetChild(1);
                Slider slider = childSlider.GetComponent<Slider>();
                slider.value = float.Parse(listExcel.listProgressTema[i]) / 100f;

                if (j == 2)
                {
                    Transform child = tem.transform.GetChild(j);
                    TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                    text.text = listExcel.listProgressTema[i] + "%";
                }
            }
        }
    }
}
