using Photon.Pun;
using UnityEngine.Events;

public class CheckIfMasterClient : MonoBehaviourPunCallbacks
{
    public UnityEvent OnMasterClient;

    void Start()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 || PhotonNetwork.IsMasterClient)
        {
            OnMasterClient?.Invoke();
        }
    }
}
