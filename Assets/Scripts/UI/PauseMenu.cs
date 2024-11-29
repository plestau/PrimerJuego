using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // El menú de pausa con los botones
    public GameObject darkOverlay; // El Panel oscuro
    private bool isPaused = false;  // Estado de si el juego está pausado o no

    void Update()
    {
        // Detectar si se presionó la tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Función para pausar el juego
    void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Mostrar el menú de pausa
        darkOverlay.SetActive(true); // Activar el panel oscuro
        Time.timeScale = 0f; // Pausar el tiempo del juego
        isPaused = true;
    }

    // Función para reanudar el juego
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Ocultar el menú de pausa
        darkOverlay.SetActive(false); // Desactivar el panel oscuro
        Time.timeScale = 1f; // Reanudar el tiempo del juego
        isPaused = false;
    }

    // Función para reiniciar el nivel
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo esté en marcha
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recargar la escena actual
    }

    // Función para salir al menú principal
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo esté en marcha
        SceneManager.LoadScene("MainMenu"); // Cambiar a la escena del menú principal (asegúrate de tener una escena de menú principal)
    }
}