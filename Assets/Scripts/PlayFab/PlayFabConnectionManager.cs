using UnityEngine;
using UnityEngine.Events;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabConnectionManager : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onPlayFabConnected;

    [Header("Debug Options")]
    [SerializeField] private bool debugLogs = true;

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    private void Start()
    {
        CheckPlayFabConnection();
    }

    private void CheckPlayFabConnection()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            if (debugLogs)
            {
                Debug.Log("Ya estás conectado a PlayFab.");
            }

            // Verificar si el SO del jugador está rellenado
            if (!string.IsNullOrEmpty(playerDataSO.username) && playerDataSO.username != "Desconocido")
            {
                if (debugLogs)
                {
                    Debug.Log("El ScriptableObject del jugador ya está rellenado. No es necesario recuperar datos.");
                }
                onPlayFabConnected?.Invoke();
            }
            else
            {
                if (debugLogs)
                {
                    Debug.Log("El ScriptableObject del jugador está vacío. Recuperando datos de PlayFab...");
                }
                RetrievePlayerDataFromPlayFab();
            }

        }
        else
        {
            if (debugLogs)
            {
                Debug.Log("No estás conectado a PlayFab. El flujo del login será responsable de conectarte.");
            }
        }
    }

    private void RetrievePlayerDataFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null)
            {
                // Rellenar el SO con los datos del jugador
                playerDataSO.username = result.Data.ContainsKey("Username") ? result.Data["Username"].Value : "Desconocido";
                playerDataSO.level = result.Data.ContainsKey("Level") ? int.Parse(result.Data["Level"].Value) : 1;
                playerDataSO.points = result.Data.ContainsKey("Points") ? int.Parse(result.Data["Points"].Value) : 0;
                playerDataSO.selectedCharacterId = result.Data.ContainsKey("SelectedCharacterId") ? int.Parse(result.Data["SelectedCharacterId"].Value) : 0;

                if (debugLogs)
                {
                    Debug.Log($"Datos cargados desde PlayFab: {playerDataSO.username}, Nivel: {playerDataSO.level}, Puntos: {playerDataSO.points}, CharacterId: {playerDataSO.selectedCharacterId}");
                }

                // Invocar el evento después de cargar los datos
                onPlayFabConnected?.Invoke();
            }
            else
            {
                Debug.LogWarning("No se encontraron datos del jugador en PlayFab.");
            }
        }, OnError);
    }

    public void DisconnectFromPlayFab()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            PlayFabClientAPI.ForgetAllCredentials();
            if (debugLogs)
            {
                Debug.Log("Desconectado de PlayFab exitosamente.");
            }
        }
        else
        {
            if (debugLogs)
            {
                Debug.Log("No estás conectado a PlayFab. No es necesario desconectar.");
            }
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError($"Error al recuperar datos del jugador desde PlayFab: {error.GenerateErrorReport()}");
    }
}
