using System.Collections;
using Photon.Pun;
using UnityEngine;

public class LauncherImpostor : MonoBehaviourPunCallbacks
{
    public PhotonView TimerPrefab;
    public PhotonView GameImpostor;
    public PhotonView MyGameManager;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // Si hay dos jugadores en la sala, instanciar el GameManager y el Timer
            PhotonNetwork.Instantiate(GameImpostor.name, transform.position, Quaternion.identity);
            PhotonNetwork.Instantiate(TimerPrefab.name, transform.position, Quaternion.identity);
            PhotonNetwork.Instantiate(MyGameManager.name, transform.position, Quaternion.identity);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount > 2)
        {
            // Si hay más de dos jugadores, esperar un momento antes de intentar crear una nueva sala
            StartCoroutine(CreateNewRoomAfterDelay());
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // Si solo hay un jugador en la sala, esperar a otro jugador
            Debug.Log("Esperando a otro jugador para iniciar la partida...");
        }
    }

    private IEnumerator CreateNewRoomAfterDelay()
    {
        yield return new WaitForSeconds(1f); // Ajusta este valor según sea necesario

        // Verificar si el cliente todavía está en la sala actual antes de intentar crear una nueva sala
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // Manejar el fallo de creación de la sala
        Debug.LogWarning("Error al crear la sala: " + message);
        // Volver a intentar unirnos a una sala
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
}
