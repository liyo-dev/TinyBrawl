using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class CharacterCarousel : MonoBehaviour
{
    [Header("Characters")]
    public GameObject[] characters; // Lista de prefabs de personajes
    private int currentIndex = 0; // Índice del personaje actual
    private GameObject activeCharacter; // Personaje actualmente instanciado

    [Header("UI Elements")]
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI characterInfoText;
    public Button selectButton;
    public Button nextButton;
    public Button previousButton;

    [Header("Parent Object")]
    public Transform characterParent; // Donde instanciamos el personaje en pantalla

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    void Start()
    {
        if (characters.Length == 0)
        {
            Debug.LogError("No se han asignado personajes en el carrusel.");
            return;
        }

        // Configurar botones
        nextButton.onClick.AddListener(ShowNextCharacter);
        previousButton.onClick.AddListener(ShowPreviousCharacter);
        selectButton.onClick.AddListener(SelectCharacter);

        // Mostrar el primer personaje
        ShowCharacter(currentIndex);
    }

    private void ShowCharacter(int index)
    {
        // Destruir el personaje actual si existe
        if (activeCharacter != null)
        {
            Destroy(activeCharacter);
        }

        // Instanciar el personaje actual
        activeCharacter = Instantiate(characters[index], characterParent);
        activeCharacter.transform.localPosition = Vector3.zero; // Centrar en el parent
        activeCharacter.transform.localRotation = Quaternion.identity;

        // Actualizar la información del personaje
        characterNameText.text = characters[index].name;
        characterInfoText.text = $"Información de {characters[index].name}"; // Ajusta según tus datos
    }

    private void ShowNextCharacter()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        ShowCharacter(currentIndex);
    }

    private void ShowPreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        ShowCharacter(currentIndex);
    }

    private void SelectCharacter()
    {
        Debug.Log($"Has seleccionado: {characters[currentIndex].name}");

        // Guardar en el SO
        playerDataSO.selectedCharacterId = currentIndex;
        Debug.Log($"Personaje guardado en SO: {playerDataSO.selectedCharacterId}");

        // Guardar en PlayFab
        SaveCharacterToPlayFab(currentIndex);

        //Vuelvo a la escena User
        SceneManager.LoadScene(SceneNames.User.ToString());
    }

    private void SaveCharacterToPlayFab(int characterId)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "SelectedCharacterId", characterId.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnCharacterSavedToPlayFab, OnPlayFabError);
    }

    private void OnCharacterSavedToPlayFab(UpdateUserDataResult result)
    {
        Debug.Log("Personaje seleccionado guardado correctamente en PlayFab.");
    }

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError($"Error al guardar personaje en PlayFab: {error.GenerateErrorReport()}");
    }
}
