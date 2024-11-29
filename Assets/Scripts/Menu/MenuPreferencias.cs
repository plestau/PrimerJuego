using UnityEngine;

public class MenuPreferencias : MonoBehaviour
{
    private void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }
        AudioListener.volume = PlayerPrefs.GetFloat("musicVolume");

        if (!PlayerPrefs.HasKey("isFullScreen"))
        {
            PlayerPrefs.SetInt("isFullScreen", Screen.fullScreen ? 1 : 0);
        }
        Screen.fullScreen = PlayerPrefs.GetInt("isFullScreen") == 1;
    }
}