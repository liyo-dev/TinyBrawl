using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 2.5f, 0); // Offset para posicionar la barra de vida encima del personaje
    public Vector2 barSize = new Vector2(200, 20);

    private GameObject healthBarGO;
    private Image healthBarFill;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f; // Salud máxima
    private float currentHealth; // Salud actual

    private void Start()
    {
        // Inicializar la salud
        currentHealth = maxHealth;

        // Crear el GameObject para la barra de vida flotante
        healthBarGO = new GameObject("HealthBar");
        healthBarGO.transform.SetParent(transform); // Hacer que la barra de vida sea hija del jugador
        healthBarGO.transform.localPosition = offset; // Posicionar la barra de vida según el offset
        healthBarGO.transform.localScale = new Vector3(0.01f, 0.015f, 1f);

        // Configurar el Canvas para la barra de vida
        Canvas healthCanvas = healthBarGO.AddComponent<Canvas>();
        healthCanvas.renderMode = RenderMode.WorldSpace;
        healthCanvas.worldCamera = Camera.main;

        CanvasScaler canvasScaler = healthBarGO.AddComponent<CanvasScaler>();
        canvasScaler.dynamicPixelsPerUnit = 10f;

        // Crear el relleno de la barra de vida
        GameObject fillBar = new GameObject("Fill");
        fillBar.transform.SetParent(healthBarGO.transform);
        healthBarFill = fillBar.AddComponent<Image>();
        healthBarFill.color = Color.green;

        // Configuración del RectTransform
        RectTransform fillRect = fillBar.GetComponent<RectTransform>();
        fillRect.sizeDelta = barSize; // Aplica las dimensiones configuradas en barSize
        fillRect.localScale = Vector3.one; // Escala uniforme para evitar deformaciones
        fillRect.anchorMin = new Vector2(0, 0.5f); // Ancla inferior
        fillRect.anchorMax = new Vector2(1, 0.5f); // Ancla superior
        fillRect.pivot = new Vector2(0.5f, 0.5f); // Mantén el pivote en el centro
        fillRect.localPosition = Vector3.zero; // Centra la posición
    }

    private void LateUpdate()
    {
        if (healthBarGO != null)
        {
            // Mantener la barra de vida orientada hacia la cámara
            if (Camera.main != null)
            {
                healthBarGO.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }

    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    public void ReduceHealth(float amount)
    {
        if (amount <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Debug.Log("Jugador ha muerto.");
        }
    }

    public void RestoreHealth(float amount)
    {
        if (amount <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        Debug.Log("CurrentHealth es: " + currentHealth);
        Debug.Log("Actual porcentaje es: " + currentHealth / maxHealth);
        Debug.Log("El fillamount es: " + healthBarFill.fillAmount);

        // Actualizar el relleno de la barra de vida
        float healthPercentage = currentHealth / maxHealth;
        healthBarFill.fillAmount = healthPercentage;

        // Cambiar el color según la cantidad de vida
        if (healthPercentage > 0.5f)
        {
            healthBarFill.color = Color.green;
        }
        else if (healthPercentage > 0.2f)
        {
            healthBarFill.color = Color.yellow;
        }
        else
        {
            healthBarFill.color = Color.red;
        }
    }
}
