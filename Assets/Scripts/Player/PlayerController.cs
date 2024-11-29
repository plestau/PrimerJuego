using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    private Rigidbody2D rb;

    [Header("Player Stats")] 
    public int totalLives;

    internal int currentLives;
    public int score;

    [Header("UI")]
    public GameObject heartPrefab; // Prefab del corazón que representará las vidas
    public Transform contenedorCorazones; // Contenedor donde se instanciarán los corazones
    public Transform canvasTransform; // Transform del Canvas que debe seguir al jugador

    private bool isBlinking = false; // Flag para saber si el jugador está parpadeando

    [Header("Player Effects")]
    public float blinkDuration = 0.5f; // Duración del parpadeo
    public int blinkCount = 3; // Número de parpadeos
    private bool isInvulnerable = false; // Flag para la invulnerabilidad
    public float heartVisibleDuration = 1.5f; // Tiempo que los corazones permanecerán visibles después del parpadeo
    
    [Header("Death Settings")]
    public float deathHeight = -10f; // Altura en Y por debajo de la cual el jugador muere
    private List<GameObject> hearts = new List<GameObject>(); // Lista para almacenar las instancias de los corazones

    private SpriteRenderer spriteRenderer;

    [Header("Audio")]
    public AudioClip damageSound; // Sonido que se reproducirá cuando el jugador recibe daño
    private AudioSource audioSource; // El componente AudioSource del jugador

    private void Start()
    {
        score = 0;
        rb = GetComponent<Rigidbody2D>();
        currentLives = totalLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        SetupHeartsUI();
    }

    private void Update()
    {
        // Actualiza la posición del contenedorCorazones para que siga al jugador
        UpdateHeartsPosition();
        
        if (transform.position.y < deathHeight)
        {
            RestartLevel(); // Llamar a la función de muerte si el jugador cae por debajo de deathHeight
        }
    }

    private void UpdateHeartsPosition()
    {
        // Asegurarse de que el contenedorCorazones se mueva con el jugador
        if (contenedorCorazones != null)
        {
            contenedorCorazones.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        }
    }
    
    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
    }

    // Configuración inicial de los 3 corazones
    private void SetupHeartsUI()
    {
        // Asegurarse de que el contenedor de corazones está vacío
        foreach (Transform child in contenedorCorazones)
        {
            Destroy(child.gameObject); // Limpiar los corazones previos si los hubiera
        }

        // Definir el espacio entre los corazones (ajustar este valor según la escala de tu Canvas)
        float heartSpacing = 1f; // Espacio entre los corazones (ajusta este valor según el tamaño del corazón y la pantalla)
        float totalWidth = heartSpacing * (totalLives - 1); // El ancho total que ocuparán los corazones

        // Instanciar los corazones en la UI según el número de vidas
        for (int i = 0; i < totalLives; i++)
        {
            GameObject heart = Instantiate(heartPrefab, contenedorCorazones);
            heart.SetActive(true); // Asegurarse de que el corazón se muestre al instanciar
            hearts.Add(heart); // Agregar el corazón a la lista de corazones
            heart.SetActive(false);
            // Calcular la posición horizontal del corazón
            // El objetivo es centrarlos sobre la posición del contenedor
            float xPosition = (i * heartSpacing) - (totalWidth / 2f); // Centrar los corazones dentro del contenedor
            heart.transform.localPosition = new Vector3(xPosition, 0, 0); // Posicionar horizontalmente dentro del Canvas
        }
    }

    // Método para actualizar los iconos de corazones en la UI según el número de vidas
    internal void UpdateHeartsUI()
    {
        // Asegurarse de que el número de corazones sea igual al de las vidas actuales
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentLives)
            {
                hearts[i].SetActive(true); // Hacer visibles los corazones activos
            }
            else
            {
                hearts[i].SetActive(false); // Desactivar los corazones que ya no están activos
            }
        }
    }

    // Método que recibe el daño al chocar con un enemigo
    public void TakeDamage(int amount)
    {
        if (!isInvulnerable && currentLives > 0)
        {
            currentLives -= amount;
            UpdateHeartsUI(); // Update hearts after losing a life
            StartCoroutine(BlinkPlayer());
            StartCoroutine(ShowHeartsTemporarily()); // Show hearts temporarily
            audioSource.PlayOneShot(damageSound); // Play damage sound
        }

        if (currentLives <= 0)
        {
            // Ocultar solo el sprite del personaje
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            // Desactivar la pistola gancho y la cuerda
            GrapplingGun grapplingGun = GetComponentInChildren<GrapplingGun>();
            if (grapplingGun != null)
            {
                grapplingGun.gameObject.SetActive(false);
                if (grapplingGun.grappleRope != null)
                {
                    grapplingGun.grappleRope.enabled = false;
                }
            }

            // Desactivar colisiones y movimiento del personaje
            rb.simulated = false; // Desactiva la física del Rigidbody2D
            GetComponent<Collider2D>().enabled = false; // Desactiva el collider del jugador

            // Reiniciar el nivel después de 3 segundos
            Invoke("RestartLevel", 3f);
        }
    }

    // Corutina para hacer parpadear al jugador
    private IEnumerator BlinkPlayer()
    {
        isInvulnerable = true; // El jugador es invulnerable durante el parpadeo

        // Hacer que el jugador parpadee
        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Cambiar visibilidad del jugador
            yield return new WaitForSeconds(blinkDuration / (blinkCount * 2));
        }

        spriteRenderer.enabled = true; // Asegurarse de que el jugador vuelva a ser visible
        isInvulnerable = false; // El jugador ya no es invulnerable
    }

    internal IEnumerator ShowHeartsTemporarily()
    {
        // Mostrar únicamente los corazones correspondientes a las vidas restantes
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < currentLives); // Hacer visibles solo los corazones activos
        }

        yield return new WaitForSeconds(heartVisibleDuration); // Esperar el tiempo adicional

        // Ocultar nuevamente los corazones después del tiempo adicional
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(false); // Asegurarse de que todos los corazones se ocultan después del tiempo adicional
        }
    }

    // Método que reinicia el nivel
    private void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // Detecta la colisión con el enemigo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isBlinking)
        {
            TakeDamage(1);
        }
    }
}
