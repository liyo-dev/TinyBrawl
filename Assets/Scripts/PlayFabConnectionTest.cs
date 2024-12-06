using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabConnectionTest : MonoBehaviour
{
    void Start()
    {
        TestPlayFabConnection();
    }

    private void TestPlayFabConnection()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = System.Guid.NewGuid().ToString(), // Genera un ID �nico para la prueba
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Conexi�n a PlayFab exitosa. ID del usuario: " + result.PlayFabId);
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error al conectar a PlayFab: " + error.GenerateErrorReport());
    }
}
