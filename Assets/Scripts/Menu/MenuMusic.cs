using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private bool shouldPlayMusic = true; // Controla si la música debe sonar al regresar al menú

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject); // Destruir duplicados
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Suscribirse al evento de cambio de escena
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Desuscribirse del evento
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") // Cambia "MainMenu" por el nombre exacto de tu menú principal
        {
            if (shouldPlayMusic && !GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().Play(); // Solo reproducir si no está sonando
            }
        }
        else if (scene.name == "Nivel1") // Cambia "GameScene" por el nombre exacto de tu nivel 1
        {
            GetComponent<AudioSource>().Stop(); // Detener la música en el juego
        }
    }

    // Método público para controlar si la música debe sonar
    public void SetShouldPlayMusic(bool value)
    {
        shouldPlayMusic = value;
    }
}