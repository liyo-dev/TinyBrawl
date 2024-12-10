using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayFabLoginManager : MonoBehaviour
{
    [SerializeField] private GameObject loader;

    [Header("Login UI")]
    [SerializeField] private TMP_InputField loginInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private GameObject loginPanel;

    [Header("Register UI")]
    [SerializeField] private TMP_InputField registerEmailInput;
    [SerializeField] private TMP_InputField registerPasswordInput;
    [SerializeField] private TMP_InputField registerUsernameInput;
    [SerializeField] private GameObject registerPanel;

    [Header("Feedback")]
    [SerializeField] private TMP_Text feedbackText;

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    public void Login()
    {
        string input = loginInput.text;
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Por favor, llena todos los campos.";
            return;
        }

        loader.SetActive(true); // Activar el loader al iniciar el proceso de login

        if (input.Contains("@"))
        {
            LoginWithEmail(input, password);
        }
        else
        {
            LoginWithUsername(input, password);
        }
    }

    private void LoginWithEmail(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    private void LoginWithUsername(string username, string password)
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }

    public void Register()
    {
        string email = registerEmailInput.text;
        string password = registerPasswordInput.text;
        string username = registerUsernameInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            feedbackText.text = "Por favor, llena todos los campos.";
            return;
        }

        loader.SetActive(true); // Activar el loader al iniciar el proceso de registro

        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        feedbackText.text = "Login exitoso. Bienvenido de nuevo.";
        Debug.Log("Login exitoso.");
        GetAccountInfo();
        LoadPlayerData();
    }

    private void GetAccountInfo()
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, result =>
        {
            if (result.AccountInfo != null && result.AccountInfo.Username != null)
            {
                playerDataSO.username = result.AccountInfo.Username;
                Debug.Log($"Username recuperado: {playerDataSO.username}");
            }
            else
            {
                Debug.LogWarning("No se encontró un username en la cuenta.");
                playerDataSO.username = "Desconocido";
            }
        }, OnError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        feedbackText.text = "Registro exitoso. Ahora puedes iniciar sesión.";
        Debug.Log("Registro exitoso.");
        playerDataSO.username = registerUsernameInput.text;
        SaveInitialPlayerData();
        ShowLoginPanel();
        loader.SetActive(false); // Desactivar el loader tras completar el registro
    }

    private void SaveInitialPlayerData()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Level", "1" },
                { "Points", "0" },
                { "SelectedCharacterId", "0" } // ID inicial del personaje
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("Datos iniciales guardados correctamente.");
        }, OnError);
    }

    private void LoadPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null)
            {
                playerDataSO.level = result.Data.ContainsKey("Level") ? int.Parse(result.Data["Level"].Value) : 1;
                playerDataSO.points = result.Data.ContainsKey("Points") ? int.Parse(result.Data["Points"].Value) : 0;
                playerDataSO.selectedCharacterId = result.Data.ContainsKey("SelectedCharacterId") ? int.Parse(result.Data["SelectedCharacterId"].Value) : 0;

                Debug.Log($"Datos cargados: Username: {playerDataSO.username}, Nivel: {playerDataSO.level}, Puntos: {playerDataSO.points}, CharacterId: {playerDataSO.selectedCharacterId}");

                SceneManager.LoadScene(SceneNames.User.ToString());
            }
            else
            {
                Debug.Log("No se encontraron datos del jugador.");
                feedbackText.text = "No se encontraron datos del jugador.";
            }

            loader.SetActive(false); // Desactivar el loader tras cargar los datos
        }, OnError);
    }

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    private void OnError(PlayFabError error)
    {
        feedbackText.text = "Error: " + error.GenerateErrorReport();
        Debug.LogError("Error: " + error.GenerateErrorReport());
        loader.SetActive(false); // Desactivar el loader en caso de error
    }
}
