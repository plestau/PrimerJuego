using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public Tutorial_GrapplingRope grappleRope;

    [Header("Layers Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private int grappableLayerNumber = 9;

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;
    private Vector2 savedVelocity; // Variable para almacenar el impulso
    
    [Header("Audio")]
    public AudioSource audioSource;  // Referencia al componente AudioSource
    public AudioClip grappleSound;   // Sonido del gancho

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistnace = 20;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
            PlayGrappleSound();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
                ApplySwingingMovement();
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true);
            }

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (launchType == LaunchType.Transform_Launch)
                {
                    Vector2 firePointDistnace = firePoint.position - gunHolder.localPosition;
                    Vector2 targetPos = grapplePoint - firePointDistnace;
                    gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            // Guardamos la velocidad actual en el momento de soltar la cuerda
            savedVelocity = m_rigidbody.velocity;

            // Desactivar la cuerda
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;

            // Asignar el impulso guardado a la velocidad del Rigidbody para conservar el momentum
            m_rigidbody.velocity = savedVelocity;

            m_rigidbody.gravityScale = 1; // Restauramos la gravedad
        }
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
        }
    }
    
    void ApplySwingingMovement()
    {
        float horizontalSwingDirection = Input.GetAxis("Horizontal");
        float verticalSwingDirection = Input.GetAxis("Vertical");
    
        Vector2 force = new Vector2(horizontalSwingDirection * launchSpeed * 5, verticalSwingDirection * launchSpeed * 5);
        m_rigidbody.AddForce(force, ForceMode2D.Force);
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
    
        // Hacemos un raycast múltiple en la dirección indicada, con el rango de distancia máximo
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, distanceVector.normalized, maxDistnace);

        // Recorremos todos los resultados y encontramos el primer colisionador válido
        foreach (RaycastHit2D hit in hits)
        {
            // Ignoramos los colisionadores que son Trigger
            if (hit.collider.isTrigger) continue;
            if (hit.collider.GetComponent<Rigidbody2D>() != null) continue;

            // Comprobamos que el objeto esté en la capa correcta o que se permita engancharse a todos los objetos
            if (hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll)
            {
                // Establecemos el punto de enganche solo si es un colisionador válido
                grapplePoint = hit.point;
                grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                grappleRope.enabled = true;
                return; // Salimos del bucle una vez que encontramos el primer colisionador válido
            }
        }
    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;
        m_springJoint2D.connectedAnchor = grapplePoint;
        m_springJoint2D.frequency = 0.5f; // Frecuencia baja para simular oscilación suave
        m_springJoint2D.dampingRatio = 0.1f; // Amortiguación baja para balanceo
        m_springJoint2D.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistnace);
        }
    }

    void PlayGrappleSound()
    {
        if (audioSource != null && grappleSound != null)
        {
            audioSource.PlayOneShot(grappleSound);  // Reproduce el sonido una sola vez
        }
    }
}
