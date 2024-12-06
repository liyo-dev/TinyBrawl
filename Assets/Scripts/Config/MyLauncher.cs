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
        if (!PhotonNetwork.IsMasterClient) return;

        //Solo el maestro instancia los juegos

        if (ServiceLocator.GetService<LocalOnlineOption>().IsImpostor())
        {
            PhotonNetwork.Instantiate(GameImpostor.name, transform.position, Quaternion.identity);
        }
        else if (ServiceLocator.GetService<LocalOnlineOption>().IsBurguer())
        {
            PhotonNetwork.Instantiate(GameBurguer.name, transform.position, Quaternion.identity);
        }
        else if (ServiceLocator.GetService<LocalOnlineOption>().IsFishing())
        {
            PhotonNetwork.Instantiate(GameFishing.name, transform.position, Quaternion.identity);
        }

        PhotonNetwork.Instantiate(MyGameManager.name, transform.position, Quaternion.identity);

        PhotonNetwork.Instantiate(TimerPrefab.name, transform.position, Quaternion.identity);
    }
}
