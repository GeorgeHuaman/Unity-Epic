using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Brightness : MonoBehaviour
{
    public Image brightnessPanel;
    public TextMeshProUGUI brightnessText;

    public float brightnessValue = 1f;
    public float step = 0.05f;

    [Range(0f, 1f)]
    public float maxDarkness = 0.8f;

    void Start()
    {
        brightnessValue = PlayerPrefs.GetFloat("brightness", 1f);
        brightnessValue = Mathf.Clamp01(brightnessValue);
        ApplyBrightness();
    }

    public void IncreaseBrightness()
    {
        brightnessValue += step;
        brightnessValue = Mathf.Clamp01(brightnessValue);
        ApplyBrightness();
    }

    public void DecreaseBrightness()
    {
        brightnessValue -= step;
        brightnessValue = Mathf.Clamp01(brightnessValue);
        ApplyBrightness();
    }

    private void ApplyBrightness()
    {
        float darknessAlpha = Mathf.Lerp(0f, maxDarkness, 1f - brightnessValue);
        Color color = brightnessPanel.color;
        brightnessPanel.color = new Color(color.r, color.g, color.b, darknessAlpha);

        PlayerPrefs.SetFloat("brightness", brightnessValue);
        UpdateUI();
    }

    private void UpdateUI()
    {
        int percentage = Mathf.RoundToInt(brightnessValue * 100f);
        brightnessText.text = percentage + "%";
    }
}

