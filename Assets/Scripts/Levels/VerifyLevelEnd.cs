using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerifyLevelEnd : MonoBehaviour
{
    private SystemProgressLevel SystemProgressLevel;
    private void Awake()
    {
        SystemProgressLevel = FindAnyObjectByType<SystemProgressLevel>();
    }

    public void EndLevel()
    {
        SystemProgressLevel.LevelEnd();
    }
}
