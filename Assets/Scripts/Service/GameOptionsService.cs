using EasyTransition;
using Photon.Pun;
using Service;
using System;
using UnityEngine.SceneManagement;

public class GameOptionsService : MonoBehaviourPunCallbacks
{
    private LocalOnlineOption localOnlineOption;

    public TransitionSettings transition;
    private void Start()
    {
        localOnlineOption = ServiceLocator.GetService<LocalOnlineOption>();
    }
    public void OnlineMode()
    {
        localOnlineOption.SetOnlineGame();
    }

    public void LocalMode()
    {
        localOnlineOption.SetLocalGame();
    }

    public void Impostor()
    {
        localOnlineOption.SetImpostorGame();

        photonView.RPC(nameof(SyncImpostor), RpcTarget.Others);

    }

    public void Burguer()
    {
        localOnlineOption.SetBurguerGame();

        photonView.RPC(nameof(SyncBurguer), RpcTarget.Others);
    }

    public void Fishing()
    {
        localOnlineOption.SetFishingGame();

        photonView.RPC(nameof(SyncFishing), RpcTarget.Others);
    }

    public void World()
    {
        localOnlineOption.SetWorldGame();

        photonView.RPC(nameof(SyncWorld), RpcTarget.Others);
    }

    public void Maze()
    {
        localOnlineOption.SetMazeGame();

        photonView.RPC(nameof(SyncMaze), RpcTarget.Others);
    }

    public void LoadGame()
    {
        photonView.RPC(nameof(SyncLoadGame), RpcTarget.All);
    }

    public void SetNoLogin()
    {
        localOnlineOption.NoLogin = true;
    }

    public void ResetNoLogin()
    {
        localOnlineOption.NoLogin = false;
    }

    [PunRPC]
    public void SyncImpostor()
    {
        localOnlineOption.SetImpostorGame();
    }

    [PunRPC]
    public void SyncBurguer()
    {
        localOnlineOption.SetBurguerGame();
    }

    [PunRPC]
    private void SyncFishing()
    {
        localOnlineOption.SetFishingGame();
    }

    [PunRPC]
    private void SyncWorld()
    {
        localOnlineOption.SetWorldGame();
    }

    [PunRPC]
    private void SyncMaze()
    {
        localOnlineOption.SetMazeGame();
    }

    [PunRPC]
    public void SyncLoadGame()
    {
        ServiceLocator.GetService<TransitionManager>().Transition(SceneNames.Game.ToString(), transition, 0);
    }


}
