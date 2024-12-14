using Photon.Pun;
using UnityEngine;

public class Portal : MonoBehaviourPunCallbacks
{
    [Header("Portal Settings")]
    [SerializeField] private string waitingRoomSceneName = "WaitingRoom"; // Nombre de la sala de espera

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra al portal es un jugador
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log($"Jugador {other.name} ha entrado al portal.");

            PopUp.Instance.Show("Entrar al laberinto", "¿Estás seguro de continuar?", TransferToWaitingRoom, OnNoAction);
        }
    }

    public void TransferToWaitingRoom()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // El primer jugador que entra se convierte en el maestro
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Soy el maestro del juego.");
            }

            // Cambiar a la escena de la sala de espera
            PhotonNetwork.LoadLevel(waitingRoomSceneName);
        }
        else
        {
            Debug.LogWarning("No estás conectado a Photon o no estás en una sala.");
        }
    }

    private void OnNoAction()
    {
        Debug.Log("El jugador seleccionó 'No'.");
    }
}
