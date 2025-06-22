using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointInterest : MonoBehaviour
{
    public Image panelBackground;
    public Image panelFront;
    public TextMeshProUGUI text;
    public TextMeshProUGUI textTitule;
    public Color panelColorBackground;
    public Color panelColorFront;
    [TextArea(1,10)]public string description;
    public string titule;
    public float range;
    [HideInInspector] public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    private void OnValidate()
    {
        if (panelBackground != null)
            panelBackground.color = panelColorBackground;

        if (panelFront != null)
            panelFront.color = panelColorFront;

        if (text != null)
            text.text = description;
        if (textTitule != null)
            textTitule.text = titule;
    }
    // Update is called once per frame
    void Update()
    {
        if (RangePlayer())
        {
            panelBackground.gameObject.SetActive(true);
        }
        else
        { panelBackground.gameObject.SetActive(false); }
    }
    public bool RangePlayer()
    {
        Vector3 horizontalDistance = new Vector3(
            transform.position.x - player.transform.position.x,
            0,
            transform.position.z - player.transform.position.z
        );

        float distance = horizontalDistance.magnitude;
        return distance <= range;
    }
}
