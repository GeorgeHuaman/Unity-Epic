using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.Video;
public class FileExplorer : MonoBehaviour
{
    private ExtensionFilter[] extensions = new[]
    {
        new ExtensionFilter("mp4"),
        new ExtensionFilter("Other", "*")
    };
    public VideoPlayer videoPlayer;
    private void Awake()
    {
        SelecVideoMobile();
    }
    private void SelectVideoPC()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        Debug.Log(paths[0]);
        string path = paths.Length > 0 ? paths[0] : null;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "file:///" + path.Replace("\\", "/");
    }
    private void SelecVideoMobile()
    {
        NativeGallery.GetVideoFromGallery((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                // Play the selected video
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = "file://" + path;
            }
        }, "Select a video");
    }

}
