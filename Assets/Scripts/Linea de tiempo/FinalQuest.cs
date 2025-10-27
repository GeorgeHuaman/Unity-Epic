using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalQuest : MonoBehaviour
{
    public List<FragLinea> objects = new List<FragLinea>();
    public Mission interactFinal;
    public static FinalQuest instance;
    private void Start()
    {
        instance = this;
    }
    public void AreAllComplete()
    {
        foreach (FragLinea obj in objects)
        {
            if (obj == null) continue;

            if (!obj.completed)
                return;
        }

        interactFinal.tasks[0].CompleteTask();
    }
}
