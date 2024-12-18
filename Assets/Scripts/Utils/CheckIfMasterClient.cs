using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [Header("Eventos")]
    public UnityEvent OnFirstPlayerEnterd; // Evento cuando el primer jugador entra en la sala

    private bool firstPlayerNotified = false; // Para asegurar que solo se emite una vez

    /// <summary>
    /// Este método se llama cuando un jugador entra en la sala.
    /// </summary>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // Si no se ha notificado aún y este jugador es el primero en entrar
        if (!firstPlayerNotified && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            firstPlayerNotified = true;
            OnFirstPlayerEnterd?.Invoke();
        }
    }

    /// <summary>
    /// Este método también se ejecuta al unirse a la sala, en caso de que este jugador sea el primero.
    /// </summary>
    public override void OnJoinedRoom()
    {
        // Verificar si este jugador es el primero al unirse
        if (!firstPlayerNotified && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            firstPlayerNotified = true;
            OnFirstPlayerEnterd?.Invoke();
        }
    }
}
