using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UserSceneManager : MonoBehaviour
{
    [Header("User Info UI")]
    [SerializeField] private TMP_Text profileNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private Image selectedCharacterImage;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button chooseCharacterButton;
    [SerializeField] private Button equipCharacterButton;
    [SerializeField] private Button shopButton;

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    void Start()
    {
        // Desactiva los botones hasta que los datos sean cargados
        playButton.interactable = false;
        chooseCharacterButton.interactable = false;
        equipCharacterButton.interactable = false;
        shopButton.interactable = false;

        // Cargar datos del usuario desde el ScriptableObject
        LoadUserData();

        // Configurar listeners de botones
        playButton.onClick.AddListener(OnPlayButtonClicked);
        chooseCharacterButton.onClick.AddListener(OnChooseCharacterButtonClicked);
        equipCharacterButton.onClick.AddListener(OnEquipCharacterButtonClicked);
        shopButton.onClick.AddListener(OnShopButtonClicked);
    }

    private void LoadUserData()
    {
        // Cargar datos del ScriptableObject
        if (playerDataSO != null)
        {
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("PlayerDataSO no está asignado.");
        }
    }

    private void UpdateUI()
    {
        // Actualizar la UI con los datos del ScriptableObject
        profileNameText.text = $"Perfil: {playerDataSO.username}";
        levelText.text = $"Nivel: {playerDataSO.level}";
        pointsText.text = $"Puntos: {playerDataSO.points}";

        // Cargar y mostrar el sprite del personaje seleccionado
        string characterSpritePath = $"Characters/Character_{playerDataSO.selectedCharacterId}";
        Sprite characterSprite = Resources.Load<Sprite>(characterSpritePath);

        if (characterSprite != null)
        {
            selectedCharacterImage.sprite = characterSprite;
        }
        else
        {
            Debug.LogWarning($"No se encontró el sprite en la ruta: {characterSpritePath}");
        }

        // Habilitar los botones ahora que los datos están cargados
        playButton.interactable = true;
        chooseCharacterButton.interactable = true;
        equipCharacterButton.interactable = true;
        shopButton.interactable = true;
    }

    private void OnPlayButtonClicked()
    {
        Debug.Log("Jugar presionado.");
        // Cargar la escena de juego
        SceneManager.LoadScene(SceneNames.SingleOrMultiPlayer.ToString());
    }

    private void OnChooseCharacterButtonClicked()
    {
        Debug.Log("Elegir Personaje presionado.");
        // Cargar la escena de selección de personajes
        SceneManager.LoadScene(SceneNames.ChoosePlayer.ToString());
    }

    private void OnEquipCharacterButtonClicked()
    {
        Debug.Log("Equipar Personaje presionado.");
        // Guardar los datos seleccionados en el ScriptableObject
        SaveCharacterData();
    }

    private void OnShopButtonClicked()
    {
        Debug.Log("Tienda presionada.");
        // Cargar la escena de la tienda
        SceneManager.LoadScene("Shop");
    }

    private void SaveCharacterData()
    {
        // Aquí puedes implementar cualquier lógica para modificar los datos del ScriptableObject
        Debug.Log("Datos del personaje actualizados en el ScriptableObject.");
    }
}
