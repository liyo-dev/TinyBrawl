using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class CheckIfMasterClient : MonoBehaviourPunCallbacks
{
    public UnityEvent OnMasterClient;

    public UnityEvent OnClient;

    void Start()
    {
        // Comprobación inicial en el cliente actual
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Soy el Master Client");
            OnMasterClient?.Invoke();
        }
        else
        {
            Debug.Log("No soy el Master Client");
            OnClient?.Invoke();
        }
    }

    // Callback para manejar cambios de Master Client
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("He sido asignado como el nuevo Master Client");
            OnMasterClient?.Invoke();
        }
        else
        {
            Debug.Log($"El nuevo Master Client es: {newMasterClient.NickName}");
            OnClient?.Invoke();
        }
    }
}
