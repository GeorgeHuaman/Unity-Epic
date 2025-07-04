using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public TextMeshProUGUI volumeText;
    //public Image muteImage;
    public float volumeValue = 1f;
    public float step = 0.05f; // Lo cambia de 5 en 5

    void Start()
    {
        volumeValue = PlayerPrefs.GetFloat("volumenAudio", 1f);
        volumeValue = Mathf.Clamp01(volumeValue);
        ApplyVolume();
    }

    public void IncreaseVolume()
    {
        volumeValue += step;
        volumeValue = Mathf.Clamp01(volumeValue);
        ApplyVolume();
    }

    public void DecreaseVolume()
    {
        volumeValue -= step;
        volumeValue = Mathf.Clamp01(volumeValue);
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        AudioListener.volume = volumeValue;
        PlayerPrefs.SetFloat("volumenAudio", volumeValue);
        UpdateUI();
    }

    private void UpdateUI()
    {
        int percentage = Mathf.RoundToInt(volumeValue * 100f);
        volumeText.text = percentage + "%";
        //muteImage.enabled = (percentage == 0);
    }
}
