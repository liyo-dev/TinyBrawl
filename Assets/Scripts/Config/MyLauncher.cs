using Photon.Pun;
using Service;
using UnityEngine;

public class MyLauncher : MonoBehaviourPunCallbacks
{
    public PhotonView GameImpostor;
    public PhotonView GameBurguer;
    public PhotonView GameFishing;
    public PhotonView TimerPrefab;
    public PhotonView MyGameManager;

    void Start()
    {
        if (ServiceLocator.GetService<LocalOnlineOption>().IsImpostor() && PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate(GameImpostor.name, transform.position, Quaternion.identity);
        }
        else if (ServiceLocator.GetService<LocalOnlineOption>().IsBurguer() && PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate(GameBurguer.name, transform.position, Quaternion.identity);
        }
        else if (ServiceLocator.GetService<LocalOnlineOption>().IsFishing() && PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate(GameFishing.name, transform.position, Quaternion.identity);
        }

        if (PhotonNetwork.CurrentRoom.Players.ContainsKey(1) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate(MyGameManager.name, transform.position, Quaternion.identity);
            
            PhotonNetwork.Instantiate(TimerPrefab.name, transform.position, Quaternion.identity);
        }

    }
}
