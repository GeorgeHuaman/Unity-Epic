using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using SFB;

public class FileExplorer : MonoBehaviour
{
    private ExtensionFilter[] extensions = new[]
    {
        new ExtensionFilter("mp4"),
        new ExtensionFilter("Other", "*")
    };

    public VideoPlayerManager videoManager;


    public void SelectVideo()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        SelectVideoPC();
#elif UNITY_ANDROID || UNITY_IOS
        SelectVideoMobile();
#else
        Debug.LogWarning("Plataforma no soportada para selección de video");
#endif
    }

    private void SelectVideoPC()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (paths.Length > 0)
        {
            string path = paths[0];
            Debug.Log("Selected video: " + path);

            videoManager.videoPlayer.source = VideoSource.Url;
            videoManager.videoPlayer.url = "file:///" + path.Replace("\\", "/");

            PrepareAndPlay();
        }
    }

    private void SelectVideoMobile()
    {
        NativeGallery.GetVideoFromGallery((path) =>
        {
            if (path != null)
            {
                Debug.Log("Video path: " + path);

                videoManager.videoPlayer.source = VideoSource.Url;
                videoManager.videoPlayer.url = "file://" + path;

                PrepareAndPlay();
            }
        }, "Select a video");
    }

    private void PrepareAndPlay()
    {
        videoManager.videoPlayer.Prepare();
        videoManager.videoPlayer.prepareCompleted += (vp) =>
        {
            videoManager.PlayLoadVideo();
        };
    }
}
