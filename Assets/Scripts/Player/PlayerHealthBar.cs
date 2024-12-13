using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealthBar : MonoBehaviourPun, IPunObservable
{
    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 2.5f, 0); // Offset para posicionar la barra de vida encima del personaje
    public Vector2 barSize = new Vector2(200, 20);

    private GameObject healthBarGO;
    private Image healthBarFill;
    private RectTransform fillRect;

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
        healthBarGO.transform.localScale = new Vector3(0.005f, 0.015f, 1f);

        // Configurar el Canvas para la barra de vida
        Canvas healthCanvas = healthBarGO.AddComponent<Canvas>();
        healthCanvas.renderMode = RenderMode.WorldSpace;
        healthCanvas.worldCamera = Camera.main;

        CanvasScaler canvasScaler = healthBarGO.AddComponent<CanvasScaler>();
        canvasScaler.dynamicPixelsPerUnit = 10f;
        // Crear el relleno de la barra de vida
        GameObject fillBarBG = new GameObject("Fill");
        fillBarBG.transform.SetParent(healthBarGO.transform);
        healthBarFill = fillBarBG.AddComponent<Image>();
        healthBarFill.color = Color.grey;

        // Configuración del RectTransform
        RectTransform fillRectBG = fillBarBG.GetComponent<RectTransform>();
        fillRectBG.sizeDelta = barSize; // Aplica las dimensiones configuradas en barSize
        fillRectBG.localScale = Vector3.one; // Escala uniforme para evitar deformaciones
        fillRectBG.anchorMin = new Vector2(0, 0.5f); // Ancla inferior
        fillRectBG.anchorMax = new Vector2(1, 0.5f); // Ancla superior
        fillRectBG.pivot = new Vector2(0.5f, 0.5f); // Mantén el pivote en el centro
        fillRectBG.localPosition = Vector3.zero; // Centra la posición


        // Crear el relleno de la barra de vida
        GameObject fillBar = new GameObject("Fill");
        fillBar.transform.SetParent(healthBarGO.transform);
        healthBarFill = fillBar.AddComponent<Image>();
        healthBarFill.color = Color.green;

        // Configuración del RectTransform
        fillRect = fillBar.GetComponent<RectTransform>();
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

        if (photonView.IsMine)
        {
            if (amount <= 0)
            {
                currentHealth = 0;

                UpdateHealthBar();

                photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth);

                return;
            }

            currentHealth = amount;

            UpdateHealthBar();

            photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth);

            if (currentHealth <= 0)
            {
                Debug.Log("Jugador ha muerto.");
            }
        }
    }

    public void RestoreHealth(float amount)
    {
        if (amount <= 0) return;

        if (photonView.IsMine)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            UpdateHealthBar();

            photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth);
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;
        // healthBarFill.fillAmount = healthPercentage;
        fillRect.localScale = new Vector3(healthPercentage, fillRect.localScale.y);

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

    [PunRPC]
    public void SyncHealth(float health)
    {
        currentHealth = health;
        UpdateHealthBar();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*  if (stream.IsWriting)
          {
              // Enviar datos a otros jugadores
              stream.SendNext(currentHealth);
          }
          else
          {
              // Recibir datos de otros jugadores
              currentHealth = (float)stream.ReceiveNext();
              UpdateHealthBar();
          }*/
    }
}
