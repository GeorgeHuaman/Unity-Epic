using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RepositionItems : MonoBehaviour
{
   public List<GameObject> items = new List<GameObject>();
    public GameObject replacement;
    public TextMeshProUGUI textMeshPro;
    private void Start()
    {
        textMeshPro.text = replacement.name;
        for (int i = 0; i < items.Count; i++)
        {
            Instantiate(replacement, items[i].transform);
        }
    }
}
