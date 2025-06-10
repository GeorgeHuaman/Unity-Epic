using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QualityGraphics : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public int quality;
    void Start()
    {
        quality = PlayerPrefs.GetInt("intQuality", 3);
        dropdown.value = quality;

    }

    public void SetQuality()
    {
        QualitySettings.SetQualityLevel(dropdown.value);
        PlayerPrefs.SetInt("intQuality", dropdown.value);
        quality = dropdown.value;
    }
}
