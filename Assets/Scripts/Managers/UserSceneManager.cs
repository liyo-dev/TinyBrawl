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
    [SerializeField] private Transform characterPosition; // Donde se instanciará el personaje

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button chooseCharacterButton;
    [SerializeField] private Button equipCharacterButton;
    [SerializeField] private Button shopButton;

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    [Header("Characters")]
    [SerializeField] private GameObject[] characters; // Lista de prefabs de personajes

    private GameObject activeCharacter; // Personaje actualmente instanciado

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
        // Validar que el ScriptableObject y los personajes estén configurados
        if (playerDataSO == null)
        {
            Debug.LogWarning("PlayerDataSO no está asignado.");
            return;
        }

        if (characters == null || characters.Length == 0)
        {
            Debug.LogWarning("No hay personajes asignados en la lista.");
            return;
        }

        // Cargar los datos del SO y mostrar el personaje seleccionado
        UpdateUI();
        ShowSelectedCharacter(playerDataSO.selectedCharacterId);
    }

    private void ShowSelectedCharacter(int characterId)
    {
        // Validar el índice del personaje
        if (characterId < 0 || characterId >= characters.Length)
        {
            Debug.LogWarning($"Índice de personaje no válido: {characterId}. Mostrando el primero por defecto.");
            characterId = 0;
        }

        // Destruir el personaje activo si existe
        if (activeCharacter != null)
        {
            Destroy(activeCharacter);
        }

        // Instanciar el personaje seleccionado
        activeCharacter = Instantiate(characters[characterId], characterPosition);
        activeCharacter.transform.localPosition = Vector3.zero; // Centrar en el padre
        activeCharacter.transform.localRotation = Quaternion.identity; // Resetear rotación
    }

    private void UpdateUI()
    {
        // Actualizar la UI con los datos del ScriptableObject
        profileNameText.text = $"Perfil: {playerDataSO.username}";
        levelText.text = $"Nivel: {playerDataSO.level}";
        pointsText.text = $"Puntos: {playerDataSO.points}";

        // Habilitar los botones ahora que los datos están cargados
        playButton.interactable = true;
        chooseCharacterButton.interactable = true;
        equipCharacterButton.interactable = true;
        shopButton.interactable = true;
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.SingleOrMultiPlayer.ToString());
    }

    private void OnChooseCharacterButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.ChoosePlayer.ToString());
    }

    private void OnEquipCharacterButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.Inventory.ToString());
    }

    private void OnShopButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.Shop.ToString());
    }
}
