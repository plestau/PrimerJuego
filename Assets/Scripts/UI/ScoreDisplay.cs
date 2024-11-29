using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI temporizador;
    private float timeElapsed = 0f;  // Tiempo transcurrido desde que comenzó el nivel
    private bool isTimerRunning = true;  // Variable para controlar si el temporizador está activo
    public TextMeshProUGUI levelText;  // Texto que muestra el nivel (Nivel 1)
    public TextMeshProUGUI scoreText;  // Texto para mostrar la puntuación
    public TextMeshProUGUI bestTime;
    public TextMeshProUGUI finalUnlocked;  // Texto para mostrar si se ha desbloqueado el final
    public TextMeshProUGUI bestTimeText;  // Texto para mostrar el mejor tiempo al finalizar el nivel
    public string levelName;  // Nombre único del nivel para identificar el mejor tiempo

    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();  // Encuentra al jugador automáticamente

        // Obtener el mejor tiempo almacenado para este nivel
        float bestTimeValue = PlayerPrefs.GetFloat(levelName + "_BestTime", float.MaxValue);

        // Si existe un mejor tiempo, mostrarlo; de lo contrario, mostrar mensaje predeterminado
        if (bestTimeValue < float.MaxValue)
        {
            bestTime.text = "Mejor tiempo: " + FormatTime(bestTimeValue);
        }
        else
        {
            bestTime.text = "¡Aún no hay un mejor tiempo!";
        }

        // Iniciar las corutinas para desvanecer los textos
        StartCoroutine(FadeOutLevelText(3f));
        StartCoroutine(FadeOutBestTimeText(3f));
    
        isTimerRunning = true;
    }


    void Update()
    {
        // Actualiza el texto con el puntaje actual
        if (playerController != null)
        {
            scoreText.text = "Puntuación: " + playerController.score;  // Actualiza el puntaje en la UI
        }

        // Si el jugador llega a 5 puntos, mostrar el mensaje de "final unlocked"
        if (playerController.score >= 5)
        {
            finalUnlocked.gameObject.SetActive(true);
        }
        
        if (isTimerRunning)
        {
            // Incrementa el tiempo transcurrido cada frame
            timeElapsed += Time.deltaTime;

            // Calcular minutos, segundos y los 4 decimales
            int minutes = Mathf.FloorToInt(timeElapsed / 60);
            int seconds = Mathf.FloorToInt(timeElapsed % 60);
            float decimals = (timeElapsed * 1000) % 1000; // Obtener los 3 primeros decimales
            string formattedTime = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, decimals);

            // Mostrar el tiempo en el formato adecuado (minutos:segundos:decimales)
            temporizador.text = formattedTime;
        }
    }
    
    public void StopTimer()
    {
        isTimerRunning = false;

        // Al finalizar el nivel, verifica el mejor tiempo
        CheckBestTime();
    }

    private void CheckBestTime()
    {
        // Obtener el mejor tiempo almacenado para este nivel
        float bestTime = PlayerPrefs.GetFloat(levelName + "_BestTime", float.MaxValue);

        // Calcular la diferencia de tiempo
        float timeDifference = timeElapsed - bestTime;

        if (timeElapsed < bestTime)
        {
            // Si el tiempo actual es mejor, actualiza el mejor tiempo
            PlayerPrefs.SetFloat(levelName + "_BestTime", timeElapsed);
            bestTimeText.text = $"¡Nuevo récord! {FormatTime(timeElapsed)}";
        }
        else
        {
            // Si no es un nuevo récord, muestra el mejor tiempo con la diferencia
            bestTimeText.text = $"Mejor tiempo: {FormatTime(bestTime)}\n (+{FormatTime(timeDifference)} s)";
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        float decimals = (time * 1000) % 1000;
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, decimals);
    }

    // Coroutine para desvanecer el texto "Nivel 1"
    private IEnumerator FadeOutLevelText(float duration)
    {
        float elapsedTime = 0f;

        // Obtén el color original del texto
        Color originalColor = levelText.color;

        // Mientras el tiempo transcurra hasta el total de duración
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alphaValue = 1 - (elapsedTime / duration);  // Calcula el valor de alpha (transparencia)

            // Aplica el nuevo color con la alpha ajustada
            levelText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaValue);

            yield return null;  // Espera hasta el siguiente frame
        }

        // Asegurarse de que el texto quede completamente transparente y desactivado
        levelText.gameObject.SetActive(false);
    }
    
    private IEnumerator FadeOutBestTimeText(float duration)
    {
        float elapsedTime = 0f;

        // Obtén el color original del texto
        Color originalColor = bestTime.color;

        // Mientras el tiempo transcurra hasta el total de duración
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alphaValue = 1 - (elapsedTime / duration);  // Calcula el valor de alpha (transparencia)

            // Aplica el nuevo color con la alpha ajustada
            bestTime.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaValue);


            yield return null;  // Espera hasta el siguiente frame
        }

        // Asegurarse de que el texto quede completamente transparente y desactivado
        bestTime.gameObject.SetActive(false);
    }
}
