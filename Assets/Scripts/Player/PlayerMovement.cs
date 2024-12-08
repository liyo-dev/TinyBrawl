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

        movementJoystick = FindObjectOfType<DynamicJoystick>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (rb == null) return;

        // Leer el input del teclado o joystick
        float horizontalInput = (movementJoystick ? movementJoystick.Horizontal : 0);
        float verticalInput = (movementJoystick ? movementJoystick.Vertical : 0);

        movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Si hay movimiento, rota y mueve el personaje
        if (movementDirection.magnitude > 0.1f)
        {
            // Movimiento adelante y atrás en el eje Z del personaje
            Vector3 moveDirection = transform.forward * movementDirection.z;

            // Rotar solo en el eje Y para girar el personaje
            float rotationInput = movementDirection.x;

            if (Mathf.Abs(rotationInput) > 0.1f)
            {
                float rotationAngle = rotationInput * rotationSpeed * Time.fixedDeltaTime;
                Quaternion deltaRotation = Quaternion.Euler(0, rotationAngle, 0);
                rb.MoveRotation(rb.rotation * deltaRotation);
            }

            // Aplicar movimiento en el eje Z
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
