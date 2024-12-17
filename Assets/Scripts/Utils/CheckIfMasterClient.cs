using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class CheckIfMasterClient : MonoBehaviourPunCallbacks
{
    [Header("Eventos")]
    public UnityEvent OnMasterClient;   // Evento a ejecutar si es el MasterClient
    public UnityEvent OnNotMasterClient; // Evento opcional si no es el MasterClient

    void Start()
    {
        CheckMasterClientStatus();
    }

    /// <summary>
    /// Verifica si el jugador actual es el MasterClient y ejecuta los eventos correspondientes.
    /// </summary>
    public void CheckMasterClientStatus()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.LogWarning("No se puede comprobar el MasterClient: no estás en una sala.");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("El jugador actual es el MasterClient.");
            OnMasterClient?.Invoke();
        }
        else
        {
            Debug.Log("El jugador actual NO es el MasterClient.");
            OnNotMasterClient?.Invoke();
        }
    }

    /// <summary>
    /// Se llama automáticamente cuando hay un cambio de MasterClient.
    /// </summary>
    /// <param name="newMasterClient">El nuevo MasterClient de la sala.</param>
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.Log($"El MasterClient ha cambiado a: {newMasterClient.NickName}");
        CheckMasterClientStatus();
    }
}
