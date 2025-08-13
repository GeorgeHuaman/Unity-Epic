using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public RenderTexture renderTexture;

    public void PlayAndPause()
    {
        VerifyVideoPlayer();

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
    }

    public void Stop()
    {
        VerifyVideoPlayer();
        videoPlayer.Stop();
        renderTexture.Release();
    }

    public void PlayLoadVideo()
    {
        videoPlayer.Play();
        SetAudio();
    }

    void VerifyVideoPlayer()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("VideoPlayer no asignado.");
            return;
        }
    }

    void SetAudio()
    {
        if (videoPlayer != null && audioSource != null)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
    }

    public void CleanRenderTexture()
    {
        renderTexture.Release();
    }

    private void OnApplicationQuit()
    {
        CleanRenderTexture();
    }
}
