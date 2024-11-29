using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOpciones : MonoBehaviour
{
    [SerializeField] private Slider volumenSlider;
    [SerializeField] private Toggle fullScreenToggle; // Referencia al Toggle de pantalla completa

    private void Start()
    {
        // Cargar configuración inicial de volumen
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }

        // Cargar configuración inicial de pantalla completa
        if (!PlayerPrefs.HasKey("isFullScreen"))
        {
            PlayerPrefs.SetInt("isFullScreen", Screen.fullScreen ? 1 : 0);
        }

        LoadSettings();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumenSlider.value;
        PlayerPrefs.SetFloat("musicVolume", volumenSlider.value); // Guardar el volumen
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("isFullScreen", isFullScreen ? 1 : 0); // Guardar el estado de pantalla completa
    }

    private void LoadSettings()
    {
        // Cargar el volumen
        volumenSlider.value = PlayerPrefs.GetFloat("musicVolume");
        AudioListener.volume = volumenSlider.value;

        // Cargar y aplicar el estado de pantalla completa
        bool isFullScreen = PlayerPrefs.GetInt("isFullScreen") == 1;
        Screen.fullScreen = isFullScreen;
        fullScreenToggle.isOn = isFullScreen; // Sincronizar el estado del Toggle
    }
}