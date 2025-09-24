using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameChicken : MonoBehaviour
{
    public int[] egg;
    public List<Transform> eggGameOject = new List<Transform>();
    [HideInInspector]public List<Transform> eggGameOjects = new List<Transform>();
    public GameObject prefabEgg;
    public Interactable interactable;
    private int maxEggs;
    public int actualEggs;
    [SerializeField]private TextMeshProUGUI textEggs;
    public static GameChicken instance;
    public GameObject uiChicken;
    public Mission quest;

    private void Start()
    {
        instance = this;
    }
    public void InteractuableButton()
    {
        uiChicken.SetActive(true);
        interactable.enabled = false;
        maxEggs = egg[Random.Range(0, egg.Length)];
        quest.tasks[0].progressSteps = maxEggs; 
        eggGameOjects = eggGameOject;
       for (int i = 0; i< maxEggs;i++)
       {
            int count = Random.Range(0, eggGameOjects.Count);
            Instantiate(prefabEgg, eggGameOjects[count].transform.position, Quaternion.identity);
            eggGameOjects.Remove(eggGameOjects[count]);
       }
        UpdateText();
    }

    private void UpdateText()
    {
        textEggs.text = $"{actualEggs}/{maxEggs}";
    }
    public void AgreeEggs()
    {
        actualEggs++;
        UpdateText();
        if(actualEggs >= maxEggs)
            quest.tasks[3].CompleteTask();
    }
}
