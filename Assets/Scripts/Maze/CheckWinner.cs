using Photon.Pun;
using UnityEngine;

public class CheckWinner : MonoBehaviourPun
{
    private GameObject winnerPopUp;
    private GameObject looserPopUp;

    private void Awake()
    {
        winnerPopUp = GameObject.FindGameObjectWithTag("WinnerPopUp");
        looserPopUp = GameObject.FindGameObjectWithTag("LooserPopUp");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Mostrar pop up he ganado
            if (winnerPopUp)
            {
                winnerPopUp.GetComponent<ShowPopUpService>().Show();
            }

            if (photonView.IsMine)
            {
                photonView.RPC(nameof(SyncPlayerLoose), RpcTarget.Others);
            }
        }
    }

    [PunRPC]
    void SyncPlayerLoose()
    {
        //Mostrar pop up he perdido
        if (looserPopUp)
        {
            looserPopUp.GetComponent<ShowPopUpService>().Show();
        }

    }
}
