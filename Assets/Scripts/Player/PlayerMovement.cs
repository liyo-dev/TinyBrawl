using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Transform cameraTransform;

    private DynamicJoystick movementJoystick; // Arrastra aquí tu joystick de movimiento
    private Button shootButton;        // Arrastra el botón de disparo
    private Button specialAttackButton; // Arrastra el botón de ataque especial

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

        movementJoystick = FindAnyObjectByType<DynamicJoystick>();
        cameraTransform = Camera.main.transform;
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

        // Si hay movimiento, rotar el personaje hacia la dirección
        if (movementDirection.magnitude > 0.1f)
        {
            // Calcular la dirección relativa a la cámara
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Rotación suave hacia el ángulo objetivo
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // Dirección final del movimiento
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            // Mover al personaje
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
