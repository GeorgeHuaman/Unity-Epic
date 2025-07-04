using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressDataSystem
{
    public string[] name;
    public bool[] end;

    public ProgressDataSystem(ProgressLevelSystem system)
    {
        int count = 0;
        for (int i = 0; i < system.levels.Count; i++)
        {
            for (int j = 0; j < system.levels[i].levelDataVerify.Count; j++)
            {
                count++;
            }
        }
        end = new bool[count];
        name = new string[count];

        int k = 0;
        for (int i = 0; i < system.levels.Count; i++)
        {
            for (int j = 0; j < system.levels[i].levelDataVerify.Count; j++)
            {
                name[k + j] = system.levels[i].levelDataVerify[j].name;
                end[k+j] = system.levels[i].levelDataVerify[j].end;
            }
            k += 4;
        }
    }
}
