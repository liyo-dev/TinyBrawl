using Photon.Pun;
using UnityEngine;

public class GameSelection : MonoBehaviourPunCallbacks
{
    public GameObject Games;
    public GameObject WaitScreen;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Games.SetActive(true);
            WaitScreen.SetActive(false);
        }
        else
        {
            Games.SetActive(false);
            WaitScreen.SetActive(true);
        }
    }
}
