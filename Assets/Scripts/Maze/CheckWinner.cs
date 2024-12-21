using Photon.Pun;
using UnityEngine;

public class CheckWinner : MonoBehaviourPunCallbacks
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
            // Mostrar pop-up de victoria para el jugador ganador
            if (winnerPopUp)
            {
                winnerPopUp.GetComponent<ShowPopUpService>().Show();
                photonView.RPC(nameof(SyncPlayerLoose), RpcTarget.Others);
            }
        }
    }

    [PunRPC]
    void SyncPlayerLoose()
    {
        // Mostrar pop-up de derrota para los jugadores perdedores
        if (looserPopUp)
        {
            looserPopUp.GetComponent<ShowPopUpService>().Show();
        }
    }
}
