using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Service;

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

}
