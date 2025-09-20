using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopFarm : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Mission quest;
    public TextMeshProUGUI no;
    public TextMeshProUGUI vendido;
    public void Sell()
    {
        if(GameCow.instance.milkCount > 0)
        {
            moneyText.text = $"{GameCow.instance.milkCount * 2}";
            GameCow.instance.milkCount = 0;
            GameCow.instance.UpdateText();
            no.gameObject.SetActive(false   );
            vendido.gameObject.SetActive(true);
            quest.tasks[4].CompleteTask();
        }
        else
        {
            no.gameObject.SetActive(true);
            vendido.gameObject.SetActive(false);
        }
    }
}
