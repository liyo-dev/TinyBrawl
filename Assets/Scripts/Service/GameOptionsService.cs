using Photon.Pun;
using Service;
using UnityEngine.SceneManagement;

public class GameOptionsService : MonoBehaviourPunCallbacks
{
    private LocalOnlineOption localOnlineOption;
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

    public void LoadGame()
    {
        photonView.RPC(nameof(SyncLoadGame), RpcTarget.All);
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
    public void SyncLoadGame()
    {
        SceneManager.LoadScene(SceneNames.Game.ToString());
    }


}
