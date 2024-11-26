using System.Collections;
using Photon.Pun;
using Service;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyLauncher : MonoBehaviourPunCallbacks
{
    public PhotonView GameImpostor;
    public PhotonView GameBurguer;
    public PhotonView TimerPrefab;
    public PhotonView MyGameManager;

    void Start()
    {
        if (ServiceLocator.GetService<LocalOnlineOption>().SelectedMiniGame == Minigame.Impostor && PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate(GameImpostor.name, transform.position, Quaternion.identity);
        }
        else if (ServiceLocator.GetService<LocalOnlineOption>().SelectedMiniGame == Minigame.Burguer && PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate(GameBurguer.name, transform.position, Quaternion.identity);
        }

        PhotonNetwork.Instantiate(MyGameManager.name, transform.position, Quaternion.identity);

        PhotonNetwork.Instantiate(TimerPrefab.name, transform.position, Quaternion.identity);
    }
}
