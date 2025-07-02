using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemProgressLevel : MonoBehaviour
{
    public List<ProgressLevel> levels = new List<ProgressLevel>();
    public static SystemProgressLevel instance;
    public string currentLevelName;
    public LevelData currentLevelVersion;

    private void Start()
    {
        foreach (var levels in levels)
        {
            if (levels.levelData != null)
            {
                for (int i = 0; i < levels.levelData.versions.Count; i++)
                {
                    levels.levelDataVerify.Add(new LevelDataVerify(levels.levelData.versions[i].versionName, false));
                }
            }
        }
    }

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

    public void IdentifyLevel(string level, LevelData version)
    {
        currentLevelName = level;
        currentLevelVersion = version;
    }

    public void LevelEnd()
    {
        int bools = 0;
        for (int i = 0; i < levels.Count; i++)
        {
            for (int j = 0; j < levels[i].levelDataVerify.Count; j++)
            {
                if (levels[i].levelDataVerify[j].name == currentLevelName && levels[i].levelDataVerify[j].end)
                {
                    Debug.Log(levels[i].levelDataVerify[j].name);
                    bools = 2;
                    break;
                }
                if (levels[i].levelDataVerify[j].name == currentLevelName && !levels[i].levelDataVerify[j].end)
                {
                    levels[i].levelDataVerify[j].end = true;
                }
                if (levels[i].levelDataVerify[j].name != currentLevelName && levels[i].levelDataVerify[j].end)
                {
                    bools++;
                }
            }
        }
        if(bools ==0)
        {
            UpdateLevel();
        }
    }

    public void UpdateLevel()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            for (int j = 0; j < levels[i].levelDataVerify.Count; j++)
            {
                if (levels[i].levelDataVerify[j].name == currentLevelName)
                {
                    levels[i].currentLevel++;
                    levels[i].UpdateLevel();
                }
            }
        }
    }
}

[Serializable]
public class ProgressLevel
{
    public string name;
    public int currentLevel = 1;
    public LevelData levelData;
    public List<LevelDataVerify> levelDataVerify = new List<LevelDataVerify>();
    public Slider progressBar;
    public Text progressText;

    public ProgressLevel (string name,int currentLevel, LevelData levelData,LevelDataVerify levelDataVerify, Slider progressBar, Text progressText)
    {
        this.name = name;
        this.currentLevel = currentLevel;
        this.levelData = levelData;
        this.progressBar = progressBar;
        this.progressText = progressText;
    }

    public void UpdateLevel()
    {
        float progress = ((float)(currentLevel - 1) / 1) * 100f;

        if (progressBar != null)
            progressBar.value = progress / 100f;

        if (progressText != null)
            progressText.text = $"{progress:F1}%";
    }
}
[Serializable]
public class LevelDataVerify
{
    public string name;
    public bool end;

    public LevelDataVerify(string name, bool end)
    {
        this.name = name;
        this.end = end;
    }
}
