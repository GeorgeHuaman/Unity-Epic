using System.Collections.Generic;
using UnityEngine;

public class AnimTexture : MonoBehaviour
{
    public Material material;
    public List<Texture> textureList;
    public float frameRate = 10f;
    public bool loop = true;

    private int currentFrame;
    private float timer;
    private bool isPlaying;

    void OnEnable()
    {
        if (material == null || textureList == null || textureList.Count == 0)
        {
            isPlaying = false;
            return;
        }

        currentFrame = 0;
        timer = 0f;
        isPlaying = true;
        material.SetTexture("_BaseMap", textureList[currentFrame]);
    }

    void Update()
    {
        if (!isPlaying || material == null || textureList == null || textureList.Count == 0)
            return;

        timer += Time.deltaTime;
        float frameDuration = 1f / frameRate;

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame++;

            if (currentFrame >= textureList.Count)
            {
                if (loop)
                    currentFrame = 0;
                else
                {
                    isPlaying = false;
                    return;
                }
            }

            material.SetTexture("_BaseMap", textureList[currentFrame]);
        }
    }

    public void Play()
    {
        OnEnable();
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void ResetAnim()
    {
        isPlaying = false;
        currentFrame = 0;

        if (material != null && textureList != null && textureList.Count > 0)
            material.SetTexture("_BaseMap", textureList[0]);
    }
}