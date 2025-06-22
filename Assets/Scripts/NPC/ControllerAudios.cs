using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerAudios : MonoBehaviour
{
    public List<Dialogue> npcs = new List<Dialogue>();
    public List<AudioSource> sources;
    public Coroutine coroutine; 

    public void StopAudios()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        for (int i = 0; i < sources.Count; i++)
        {
            sources[i].Stop();
            npcs[i].startDialogue = false;

        }
    }
}
