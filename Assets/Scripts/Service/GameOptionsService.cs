using UnityEngine;
using Photon.Pun;
using Service;
using UnityEngine.SceneManagement;

public class GameOptionsService : MonoBehaviourPunCallbacks
{
    public void OnlineMode()
    {
        ServiceLocator.GetService<LocalOnlineOption>().SetOnlineGame();
    }

    public void LocalMode()
    {
        ServiceLocator.GetService<LocalOnlineOption>().SetLocalGame();
    }

    public void Impostor()
    {
        ServiceLocator.GetService<LocalOnlineOption>().SetImpostorGame();
    }

    public void Burguer()
    {
        ServiceLocator.GetService<LocalOnlineOption>().SetBurguerGame();
    }

    public void LoadGame()
    {
        photonView.RPC(nameof(SyncLoadGame), RpcTarget.All);
    }

    [PunRPC]
    public void SyncLoadGame()
    {
        SceneManager.LoadScene(SceneNames.Game.ToString());
    }


}
