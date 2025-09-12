using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAudioSimple : MonoBehaviour
{
    public AudioSource AudioSource;

    public void PlayAudio(AudioClip audio)
    {
        AudioSource.clip = audio;
        AudioSource.Play();
    }
}
