using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemProgressLevel : MonoBehaviour
{
    public List<ProgressLevel> levels = new List<ProgressLevel>();
    public static SystemProgressLevel instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        
    }
}

[Serializable]
public class ProgressLevel
{
    public string name;
    public int currentLevel = 1;
    public LevelData levelData;
    public Slider progressBar;
    public Text progressText;

    public ProgressLevel (string name,int currentLevel, LevelData levelData, Slider progressBar, Text progressText)
    {
        this.name = name;
        this.currentLevel = currentLevel;
        this.levelData= levelData;
        this.progressBar = progressBar;
        this.progressText = progressText;
    }

    public void UpdateLevel()
    {
        float progress = ((float)(currentLevel - 1) / levelData.versions.Count) * 100f;

        if (progressBar != null)
            progressBar.value = progress / 100f;

        if (progressText != null)
            progressText.text = $"{progress:F1}%";
    }
}
