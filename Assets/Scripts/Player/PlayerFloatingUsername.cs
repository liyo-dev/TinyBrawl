using UnityEngine;
using TMPro;

public class PlayerFloatingUsername : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 2f, 0); // Offset para posicionar el texto encima del personaje

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO; // Referencia al ScriptableObject que contiene el nombre

    [Header("Font Asset")]
    [SerializeField] private TMP_FontAsset fontAsset;

    private GameObject floatingText;
    private TextMeshProUGUI usernameText;

    private void Start()
    {
        if (playerDataSO == null)
        {
            Debug.LogError("PlayerDataSO no est� asignado.");
            return;
        }

        // Crear el GameObject para el texto flotante
        floatingText = new GameObject("FloatingUsername");
        floatingText.transform.SetParent(transform); // Hacer que el texto sea hijo del jugador
        floatingText.transform.localPosition = offset; // Posicionar el texto seg�n el offset

        // A�adir un componente de TextMeshPro
        usernameText = floatingText.AddComponent<TextMeshProUGUI>();
        usernameText.text = playerDataSO.username; // Establecer el nombre del jugador
        usernameText.alignment = TextAlignmentOptions.Center; // Alinear al centro
        usernameText.fontSize = 50; // Ajustar el tama�o de fuente (puedes modificarlo seg�n tu dise�o)
        usernameText.color = Color.black;
        usernameText.font = fontAsset;

        // Configurar el CanvasRenderer para que sea visible en el mundo
        Canvas floatingCanvas = floatingText.AddComponent<Canvas>();
        floatingCanvas.renderMode = RenderMode.WorldSpace;
        floatingCanvas.worldCamera = Camera.main;

        // Ajustar el RectTransform para un mejor posicionamiento
        RectTransform rectTransform = usernameText.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50); // Tama�o del �rea del texto
        rectTransform.localScale = Vector3.one * 0.01f; // Escalar para ajustarlo al tama�o del mundo
    }

    private void LateUpdate()
    {
        if (floatingText != null)
        {
            // Mantener el texto orientado hacia la c�mara
            if (Camera.main != null)
            {
                floatingText.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
}
