using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerAudio : MonoBehaviour
{
    public List<NPC> npcs = new List<NPC>();
    public List<AudioSource> sources;
    public Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
