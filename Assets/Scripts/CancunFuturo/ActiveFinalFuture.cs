using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveFinalFuture : MonoBehaviour
{
    public List<FragPosFuturo> objects = new List<FragPosFuturo>();
    public GameObject interactFinal;
    public static ActiveFinalFuture instance;
    public Mission quest;
    private void Start()
    {
        instance = this;
    }
    public void AreAllComplete()
    {
        foreach (FragPosFuturo obj in objects)
        {
            if (obj == null) continue;

            if (!obj.completed)
                return;
        }

        interactFinal.GetComponent<Animator>().SetBool("Open", true);
        quest.tasks[1].CompleteTask();
    }
}
