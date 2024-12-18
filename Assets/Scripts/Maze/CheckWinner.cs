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
            // Mostrar pop-up de victoria para el jugador ganador
            if (winnerPopUp)
            {
                winnerPopUp.GetComponent<ShowPopUpService>().Show();
                KillAllOtherPlayers(other.gameObject);
            }
        }
    }

    private void KillAllOtherPlayers(GameObject winner)
    {
        // Buscar todos los jugadores con el tag "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // Saltar al jugador ganador
            if (player == winner) continue;

            // Obtener el componente PlayerHealth y "matar" al jugador
            PhotonView playerView = player.GetComponent<PhotonView>();
            if (playerView != null)
            {
                photonView.RPC(nameof(SyncPlayerLoose), playerView.Owner, playerView.ViewID);
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
