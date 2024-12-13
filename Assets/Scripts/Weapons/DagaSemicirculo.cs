using UnityEngine;

public class DagaSemicirculo : MonoBehaviour
{
    public float attackRadius = 2f; // Radio del semic�rculo
    public float attackSpeed = 5f; // Velocidad de movimiento
    public float attackAngle = 180f; // �ngulo total del semic�rculo
    public float duration = 1f; // Duraci�n antes de destruir la daga

    private float currentAngle = 0f; // �ngulo actual
    private Transform playerTransform; // Referencia al jugador
    private bool isReturning = false; // Determina si la daga est� regresando

    private void Start()
    {
        // Encontrar el jugador por tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("No se encontr� al jugador con el tag 'Player'.");
            Destroy(gameObject);
        }

        // Destruir la daga despu�s de la duraci�n especificada
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Calcular el �ngulo de movimiento
        float step = attackSpeed * Time.deltaTime;
        currentAngle += step;

        if (currentAngle >= attackAngle && !isReturning)
        {
            // Comenzar a regresar
            isReturning = true;
        }

        // Calcular la posici�n de la daga
        float angleRadians = Mathf.Deg2Rad * (isReturning ? attackAngle - currentAngle : currentAngle);
        float x = playerTransform.position.x + Mathf.Cos(angleRadians) * attackRadius;
        float z = playerTransform.position.z + Mathf.Sin(angleRadians) * attackRadius;

        transform.position = new Vector3(x, playerTransform.position.y, z);

        // Rotar la daga sobre s� misma (opcional, para efecto visual)
        transform.Rotate(Vector3.up, attackSpeed * 10f * Time.deltaTime);
    }
}
