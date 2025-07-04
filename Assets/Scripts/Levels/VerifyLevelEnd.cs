using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerifyLevelEnd : MonoBehaviour
{
    private ProgressLevelSystem SystemProgressLevel;
    private void Awake()
    {
        SystemProgressLevel = FindAnyObjectByType<ProgressLevelSystem>();
    }

    public void EndLevel()
    {
        SystemProgressLevel.LevelEnd();
    }
}
