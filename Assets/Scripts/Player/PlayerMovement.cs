using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 30f;

    private DynamicJoystick movementJoystick;

    private Vector3 movementDirection;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un componente Rigidbody. Por favor, añade uno al jugador.");
            return;
        }

        // Restringir las rotaciones físicas en el Rigidbody
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        movementJoystick = FindObjectOfType<DynamicJoystick>();
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        if (rb == null) return;

        // Leer el input del joystick
        float horizontalInput = movementJoystick ? movementJoystick.Horizontal : 0;
        float verticalInput = movementJoystick ? movementJoystick.Vertical : 0;

        movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Si hay movimiento, rotar y mover al personaje
        if (movementDirection.magnitude > 0.1f)
        {
            // Calcular el ángulo hacia la dirección de movimiento
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;

            // Rotar suavemente hacia el ángulo deseado
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(Quaternion.Euler(0, angle, 0));

            // Mover al personaje en la dirección que está mirando
            Vector3 moveDirection = movementDirection;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
