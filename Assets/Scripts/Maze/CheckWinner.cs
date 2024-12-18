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
            }

            // Solo el jugador maestro ejecuta la lógica de "matar" a los demás
            if (PhotonNetwork.IsMasterClient)
            {
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
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
               // photonView.RPC(nameof(SyncPlayerLoose), player.GetComponent<PhotonView>().Owner);
                playerHealth.TakeDamageRPC(int.MaxValue); // Matar al jugador directamente
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
