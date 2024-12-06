using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
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

    private string profileName;
    private int level;
    private int points;
    private string characterSpritePath;

    void Start()
    {
        // Desactiva los botones hasta que los datos sean cargados
        playButton.interactable = false;
        chooseCharacterButton.interactable = false;
        equipCharacterButton.interactable = false;
        shopButton.interactable = false;

        // Cargar datos del usuario desde PlayFab
        LoadUserData();

        // Configurar listeners de botones
        playButton.onClick.AddListener(OnPlayButtonClicked);
        chooseCharacterButton.onClick.AddListener(OnChooseCharacterButtonClicked);
        equipCharacterButton.onClick.AddListener(OnEquipCharacterButtonClicked);
        shopButton.onClick.AddListener(OnShopButtonClicked);
    }

    private void LoadUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnUserDataLoaded, OnError);
    }

    private void OnUserDataLoaded(GetUserDataResult result)
    {
        if (result.Data != null)
        {
            // Cargar datos específicos del usuario desde PlayFab
            profileName = result.Data.ContainsKey("ProfileName") ? result.Data["ProfileName"].Value : "Desconocido";
            level = result.Data.ContainsKey("Level") ? int.Parse(result.Data["Level"].Value) : 1;
            points = result.Data.ContainsKey("Points") ? int.Parse(result.Data["Points"].Value) : 0;
            characterSpritePath = result.Data.ContainsKey("CharacterSprite") ? result.Data["CharacterSprite"].Value : "Characters/DefaultCharacter";

            // Actualizar la interfaz con los datos cargados
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("No se encontraron datos del usuario en PlayFab.");
        }
    }

    private void UpdateUI()
    {
        profileNameText.text = $"Perfil: {profileName}";
        levelText.text = $"Nivel: {level}";
        pointsText.text = $"Puntos: {points}";

        // Cargar y mostrar el sprite del personaje seleccionado
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
        SceneManager.LoadScene("GameScene");
    }

    private void OnChooseCharacterButtonClicked()
    {
        Debug.Log("Elegir Personaje presionado.");
        // Cargar la escena de selección de personajes
        SceneManager.LoadScene("CharacterSelection");
    }

    private void OnEquipCharacterButtonClicked()
    {
        Debug.Log("Equipar Personaje presionado.");
        // Guardar personaje seleccionado en PlayFab
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
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "CharacterSprite", characterSpritePath }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("Personaje equipado y guardado correctamente.");
        }, OnError);
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError($"Error: {error.GenerateErrorReport()}");
    }
}
