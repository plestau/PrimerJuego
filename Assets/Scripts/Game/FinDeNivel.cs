using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCompletion : MonoBehaviour
{
    public TextMeshProUGUI completionText;  // El texto de nivel completado (debe estar en una UI Text)
    public AudioClip completionSound;  // Sonido que se reproduce cuando se completa el nivel
    public TextMeshProUGUI mejorTiempo;
    public string nextSceneName;  // El nombre de la siguiente escena
    private PlayerController playerController;
    public ScoreDisplay scoreDisplay;

    private bool levelCompleted = false;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();  // Encuentra al jugador automáticamente
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController.score < 5 && !levelCompleted)
            {
                StartCoroutine(ShowMessageForSeconds("Te hacen falta 5 puntos", 2f));
            }
            else if (playerController.score >= 5 && !levelCompleted)
            {
                levelCompleted = true;
                CompleteLevel();
            }
        }
    }

    private IEnumerator ShowMessageForSeconds(string message, float seconds)
    {
        completionText.text = message;
        completionText.gameObject.SetActive(true);

        // Espera para que el mensaje se muestre durante 'seconds' segundos antes de comenzar el desvanecimiento
        yield return new WaitForSeconds(seconds);

        // Iniciar el desvanecimiento
        StartCoroutine(FadeOutText(1f)); // El texto se desvanece en 1 segundo
    }

    private IEnumerator FadeOutText(float fadeDuration)
    {
        float elapsedTime = 0f;

        // Guardamos el color original del texto
        Color originalColor = completionText.color;

        // Mientras no haya transcurrido el tiempo de desvanecimiento
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - (elapsedTime / fadeDuration); // Calculamos el valor de alpha (transparencia)

            // Cambiamos el color del texto ajustando la transparencia
            completionText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null; // Espera hasta el siguiente frame
        }

        // Asegúrate de que el texto quede completamente invisible
        completionText.gameObject.SetActive(false);
    }

    private void CompleteLevel()
    {
        // Verificar si scoreDisplay está asignado antes de llamar a StopTimer
        if (scoreDisplay != null)
        {
            scoreDisplay.StopTimer();  // Detenemos el temporizador
        }

        // Hacer al jugador invulnerable
        playerController.SetInvulnerable(true);

        // Detenemos el desvanecimiento en caso de que esté activo y restauramos la transparencia del texto
        StopAllCoroutines();
        completionText.color = new Color(completionText.color.r, completionText.color.g, completionText.color.b, 1f);

        // Configuramos el mensaje de "Felicidades"
        completionText.text = "Felicidades";
        completionText.gameObject.SetActive(true);

        mejorTiempo.gameObject.SetActive(true);

        // Reproducir el sonido de nivel completado
        AudioSource.PlayClipAtPoint(completionSound, Camera.main.transform.position);

        // Asegurarse de que el jugador no reciba daño mientras se carga la siguiente escena
        playerController.SetInvulnerable(true);

        // Esperar 3 segundos antes de cambiar de escena
        Invoke("ChangeScene", 3f);
    }

    private void ChangeScene()
    {
        // Cargar la siguiente escena
        SceneManager.LoadScene(nextSceneName);
    }
}
