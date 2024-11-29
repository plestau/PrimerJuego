using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float speed;
    public bool chase = false;
    public Transform startingPoint;
    public float patrolRange = 2.0f; // Distancia máxima de patrullaje horizontal
    private GameObject player;
    private bool movingRight = true; // Controla la dirección de patrullaje

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }
        if (chase)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
        Flip();
    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void Flip()
    {
        if (chase)
        {
            // Voltear hacia el jugador durante la persecución
            if (transform.position.x > player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        else
        {
            // Voltear según la dirección de patrullaje
            if (movingRight)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0); // Mirando a la derecha
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0); // Mirando a la izquierda
            }
        }
    }

    private void Patrol()
    {
        // Calcula el límite de patrullaje
        float leftLimit = startingPoint.position.x - patrolRange;
        float rightLimit = startingPoint.position.x + patrolRange;

        // Mueve el enemigo hacia la izquierda o derecha dentro del rango
        if (movingRight)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            if (transform.position.x >= rightLimit)
            {
                movingRight = false; // Cambia la dirección al alcanzar el límite derecho
            }
        }
        else
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            if (transform.position.x <= leftLimit)
            {
                movingRight = true; // Cambia la dirección al alcanzar el límite izquierdo
            }
        }
    }

    // Detecta la colisión con el jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // El enemigo ha colisionado con el jugador
            // Llamamos al método TakeDamage() del jugador
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(1);
            }
        }
    }
}
