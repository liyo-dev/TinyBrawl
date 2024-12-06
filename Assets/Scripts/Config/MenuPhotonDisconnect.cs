using Photon.Pun;
using UnityEngine;

public class MenuPhotonDisconnect : MonoBehaviour
{
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Desconectando de Photon...");
            PhotonNetwork.Disconnect();
        }
    }
}
