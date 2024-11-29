using UnityEngine;

public class PointsController : MonoBehaviour
{
    public float amplitude = 0.5f; // La distancia de movimiento hacia arriba y abajo
    public float frequency = 1f; // La velocidad del movimiento
    private Vector3 startPos;
    private AudioSource audioSource;

    // Referencia al script PlayerController
    public PlayerController playerController;

    void Start()
    {
        startPos = transform.position;
        audioSource = GetComponent<AudioSource>();

        // Asegurarse de que la referencia al playerController esté configurada (puede hacerse en el inspector o buscarlo en el Start)
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>(); // Si no se asigna en el Inspector, lo encuentra automáticamente.
        }
    }

    void Update()
    {
        // Calcula la nueva posición en Y utilizando una función de seno para un movimiento suave
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el objeto que entra en el trigger es el personaje
        if (other.CompareTag("Player"))
        {
            // Si el jugador toca el punto, aumentar el puntaje
            if (playerController != null)
            {
                playerController.score += 1;  // Sumar 1 al puntaje
                // si la vida del jugador es menor que el total de vidas, aumentar la vida
                if (playerController.currentLives < 5)
                {
                    playerController.currentLives += 1; // Aumentar la vida
                    playerController.UpdateHeartsUI(); // Actualizar la UI de las vidas
                    playerController.StartCoroutine(playerController.ShowHeartsTemporarily()); // Mostrar los corazones temporalmente
                }
            }

            // Reproduce el sonido y destruye el punto
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
            Destroy(gameObject);  // Destruye el objeto del punto
        }
    }
}