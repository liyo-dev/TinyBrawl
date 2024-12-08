using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 30f;

    private DynamicJoystick movementJoystick; // Arrastra aquí tu joystick de movimiento
    private Button shootButton;               // Arrastra el botón de disparo
    private Button specialAttackButton;       // Arrastra el botón de ataque especial

    private Vector3 movementDirection;
    private bool isShooting = false;
    private bool isSpecialAttacking = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un componente Rigidbody. Por favor, añade uno al jugador.");
            return;
        }

        if (shootButton != null)
            shootButton.onClick.AddListener(Shoot);

        if (specialAttackButton != null)
            specialAttackButton.onClick.AddListener(SpecialAttack);

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
        float horizontalInput = Input.GetAxis("Horizontal") + (movementJoystick ? movementJoystick.Horizontal : 0);
        float verticalInput = Input.GetAxis("Vertical") + (movementJoystick ? movementJoystick.Vertical : 0);

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

    private void Shoot()
    {
        if (isShooting) return; // Evitar que el jugador dispare mientras ya está disparando

        Debug.Log("Disparo activado");
        // Aquí puedes añadir lógica para instanciar proyectiles o disparos
        isShooting = true;
        Invoke(nameof(ResetShoot), 0.5f); // Ejemplo: cooldown de 0.5 segundos
    }

    private void ResetShoot()
    {
        isShooting = false;
    }

    private void SpecialAttack()
    {
        if (isSpecialAttacking) return; // Evitar que el jugador haga el ataque especial mientras ya está atacando

        Debug.Log("Ataque especial activado");
        // Aquí puedes añadir lógica para el ataque especial
        isSpecialAttacking = true;
        Invoke(nameof(ResetSpecialAttack), 2f); // Ejemplo: cooldown de 2 segundos
    }

    private void ResetSpecialAttack()
    {
        isSpecialAttacking = false;
    }
}
