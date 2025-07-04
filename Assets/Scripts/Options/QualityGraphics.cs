using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QualityGraphics : MonoBehaviour
{
    public TextMeshProUGUI qualityLabel;
    public int quality = 3;
    private string[] qualityNames;
    void Start()
    {
        qualityNames = QualitySettings.names;
        quality = PlayerPrefs.GetInt("intQuality", 3);
        quality = Mathf.Clamp(quality, 0, qualityNames.Length - 1);
        UpdateQualityUI();

    }

    public void IncreaseQuality()
    {
        if (quality < qualityNames.Length - 1)
        {
            quality++;
            ApplyQuality();
        }
    }

    public void DecreaseQuality()
    {
        if (quality > 0)
        {
            quality--;
            ApplyQuality();
        }
    }

    private void ApplyQuality()
    {
        QualitySettings.SetQualityLevel(quality);
        PlayerPrefs.SetInt("intQuality", quality);
        UpdateQualityUI();
    }

    private void UpdateQualityUI()
    {
        qualityLabel.text = qualityNames[quality];
    }
}
