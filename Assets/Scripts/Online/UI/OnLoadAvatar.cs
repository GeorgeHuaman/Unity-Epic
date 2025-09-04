using ReadyPlayerMe.Core.Analytics;
using ReadyPlayerMe.Samples.QuickStart;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OnLoadAvatar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI openPersonalAvatarPanelButtonText;
    [SerializeField] private Button openPersonalAvatarPanelButton;
    [SerializeField] private TMP_InputField avatarUrlField;
    [SerializeField] private GameObject avatarLoading;

    [Header("Character Managers")]
    [SerializeField] public LoaderAvatar loaderAvatar;

    private string defaultButtonText;

    private void OnEnable()
    {
        loaderAvatar = LoaderAvatar.singleton;
    }
    public void OnLoadAvatarButton()
    {
        loaderAvatar.OnLoadComplete += OnLoadComplete;
        //defaultButtonText = openPersonalAvatarPanelButtonText.text;
        SetActiveLoading(true, "Loading...");

        loaderAvatar.LoadAvatar(avatarUrlField.text);
        this.gameObject.SetActive(false);
        AnalyticsRuntimeLogger.EventLogger.LogPersonalAvatarLoading(avatarUrlField.text);
    }
    private void OnLoadComplete()
    {
        loaderAvatar.OnLoadComplete -= OnLoadComplete;
        SetActiveLoading(false, defaultButtonText);
    }
    private void SetActiveLoading(bool enable, string text)
    {
        //openPersonalAvatarPanelButtonText.text = text;
        //openPersonalAvatarPanelButton.interactable = !enable;
        //avatarLoading.SetActive(enable);
    }
}
