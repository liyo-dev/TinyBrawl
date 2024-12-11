using UnityEngine;
using UnityEngine.Events;
using PlayFab;
using PlayFab.ClientModels;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayFabAuthManager : MonoBehaviour
{
    [SerializeField] private GameObject loader;

    [Header("Events")]
    public UnityEvent onLocalDataNotFound; // Evento cuando no se encuentra el archivo local
    public UnityEvent onLoginSuccess;     // Evento cuando el login es exitoso
    public UnityEvent onLoginFailed;      // Evento cuando el login falla

    [Header("Debug Options")]
    [SerializeField] private bool debugLogs = true;

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    private string playerDataFilePath;

    private void Awake()
    {
        // Establecer la ruta del archivo JSONnad
        playerDataFilePath = Path.Combine(Application.persistentDataPath, "PlayerLoginData.json");
        Debug.Log("La ruta donde esta el fichero guardado es: " + playerDataFilePath);
    }

    private void Start()
    {
        loader.SetActive(true);
        CheckLocalDataOrLogin();
    }

    private void CheckLocalDataOrLogin()
    {
        if (File.Exists(playerDataFilePath))
        {
            if (debugLogs)
                Debug.Log("Archivo local encontrado. Intentando login automático...");

            // Leer datos del archivo JSON
            PlayerLoginData loginData = LoadPlayerDataFromFile();

            if (loginData != null)
            {
                LoginToPlayFab(loginData);
            }
            else
            {
                if (debugLogs)
                    Debug.Log("Datos locales incompletos o corruptos. Notificando evento de datos no encontrados.");

                loader.SetActive(false);
                onLocalDataNotFound?.Invoke();
            }
        }
        else
        {
            if (debugLogs)
                Debug.Log("No se encontró el archivo local de datos. Notificando evento de datos no encontrados.");

            loader.SetActive(false);
            onLocalDataNotFound?.Invoke();
        }
    }


    public void LoginToPlayFab(PlayerLoginData loginData)
    {
        string inputEmail = loginData.Email;
        string inputUser = loginData.UserName;
        string password = loginData.Password;

        if (string.IsNullOrEmpty(inputEmail) && inputEmail.Contains("@"))
        {
            LoginWithEmail(inputEmail, password);
        }
        else
        {
            LoginWithUsername(inputUser, password);
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

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login exitoso.");
        LoadPlayerData();
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
        onLoginFailed?.Invoke();
    }


    private PlayerLoginData LoadPlayerDataFromFile()
    {
        try
        {
            string json = File.ReadAllText(playerDataFilePath);
            PlayerLoginData loginData = JsonUtility.FromJson<PlayerLoginData>(json);

            if (debugLogs)
                Debug.Log("Datos de login cargados desde el archivo local correctamente.");

            return loginData;
        }
        catch (System.Exception ex)
        {
            if (debugLogs)
                Debug.LogError($"Error al leer el archivo JSON: {ex.Message}");

            return null;
        }
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
                playerDataSO.rightHandItemId = result.Data.ContainsKey("RightHandItemId") ? int.Parse(result.Data["RightHandItemId"].Value) : -1;
                playerDataSO.leftHandItemId = result.Data.ContainsKey("LeftHandItemId") ? int.Parse(result.Data["LeftHandItemId"].Value) : -1;

                Debug.Log($"Datos cargados: Username: {playerDataSO.username}, Nivel: {playerDataSO.level}, Puntos: {playerDataSO.points}, CharacterId: {playerDataSO.selectedCharacterId}");

                onLoginSuccess?.Invoke();
            }
            else
            {
                Debug.Log("No se encontraron datos del jugador.");
            }

        }, OnError);
    }
}
