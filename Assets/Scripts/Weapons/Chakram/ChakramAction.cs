using UnityEngine;

public class ChakramAction : MonoBehaviour
{
    public float orbitSpeed = 100f; // Velocidad de rotación
    public float orbitRadius = 5f; // Radio de la órbita
    public int orbitRevolutions = 3; // Número de vueltas antes de destruir

    private Transform playerTransform; // Transform del jugador
    private float currentAngle = 0f;
    private int completedRevolutions = 0;
    private bool canCheck = false;

    private void Start()
    {
        // Buscar al jugador por tag "Player"
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("No se encontró ningún jugador con el tag 'Player'. El Chakram no puede orbitar.");
            Destroy(gameObject); // Eliminar el objeto si no se encuentra un jugador
        }
    }

    public void DoStart()
    {
        canCheck = true;
    }

    private void Update()
    {
        if (!canCheck) return;
        if (playerTransform == null) return;

        // Calcular la posición de la órbita
        currentAngle += orbitSpeed * Time.deltaTime;
        float radians = currentAngle * Mathf.Deg2Rad;
        float x = playerTransform.position.x + Mathf.Cos(radians) * orbitRadius;
        float z = playerTransform.position.z + Mathf.Sin(radians) * orbitRadius;

        transform.position = new Vector3(x, playerTransform.position.y, z);

        // Rotar el chakram sobre sí mismo (opcional)
        transform.Rotate(Vector3.up, orbitSpeed * Time.deltaTime);

        // Contar las vueltas completas
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
            completedRevolutions++;

            // Destruir después de completar las vueltas configuradas
            if (completedRevolutions >= orbitRevolutions)
            {
                Destroy(gameObject);
            }
        }
    }
}
