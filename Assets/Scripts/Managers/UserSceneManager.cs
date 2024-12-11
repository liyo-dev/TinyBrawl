using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Service;

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
    private PlayerDataSO playerDataSO;

    [Header("Characters")]
    [SerializeField] private GameObject[] characters; // Lista de prefabs de personajes

    [Header("Inventory Items")]
    [SerializeField] private GameObject[] inventoryItems; // Lista de prefabs de armas

    private GameObject activeCharacter; // Personaje actualmente instanciado

    private void Start()
    {
        // Configurar listeners de botones
        playButton.onClick.AddListener(OnPlayButtonClicked);
        chooseCharacterButton.onClick.AddListener(OnChooseCharacterButtonClicked);
        equipCharacterButton.onClick.AddListener(OnEquipCharacterButtonClicked);
        shopButton.onClick.AddListener(OnShopButtonClicked);

        playerDataSO = ServiceLocator.GetService<PlayerDataService>().GetData();

        LoadUserData();
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

        EquipWeapons();
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
        Rigidbody rb = activeCharacter.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Evitar que la gravedad o las fuerzas afecten al personaje
        }
        activeCharacter.transform.localPosition = Vector3.zero; // Centrar en el padre
        activeCharacter.transform.localRotation = Quaternion.identity; // Resetear rotación
    }

    private void EquipWeapons()
    {
        // Usar GameObject.FindWithTag para encontrar las zonas de las manos
        GameObject leftHandObject = GameObject.FindWithTag("EquipLeft");
        GameObject rightHandObject = GameObject.FindWithTag("EquipRight");

        // Obtener los transform de las zonas de las manos
        Transform leftHandZone = leftHandObject != null ? leftHandObject.transform : null;
        Transform rightHandZone = rightHandObject != null ? rightHandObject.transform : null;

        if (leftHandZone != null && playerDataSO.leftHandItemId >= 0 && playerDataSO.leftHandItemId < inventoryItems.Length)
        {
            InstantiateWeapon(inventoryItems[playerDataSO.leftHandItemId], leftHandZone);
        }

        if (rightHandZone != null && playerDataSO.rightHandItemId >= 0 && playerDataSO.rightHandItemId < inventoryItems.Length)
        {
            InstantiateWeapon(inventoryItems[playerDataSO.rightHandItemId], rightHandZone);
        }
    }


    private void InstantiateWeapon(GameObject weaponPrefab, Transform handZone)
    {
        if (handZone.childCount > 0)
        {
            Destroy(handZone.GetChild(0).gameObject);
        }

        GameObject weapon = Instantiate(weaponPrefab, handZone);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
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
