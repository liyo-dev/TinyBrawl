using Photon.Pun;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    public PhotonView Player1Prefab;
    public PhotonView Player2Prefab;
    public Transform SpawnPoint_Player1;
    public Transform SpawnPoint_Player2;
    private const int MAX_PLAYERS_PER_ROOM = 3;

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
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            //CheckPlayersInRoom();
            PhotonNetwork.Instantiate(Player1Prefab.name, SpawnPoint_Player1.transform.position, Quaternion.identity);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            //CheckPlayersInRoom();
            PhotonNetwork.Instantiate(Player2Prefab.name, SpawnPoint_Player2.transform.position, Quaternion.identity);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == MAX_PLAYERS_PER_ROOM)
        {
            Debug.Log("Sala llena. Creando una nueva sala...");
            CreateNewRoom();
        }
    }

    private void CheckPlayersInRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) StartGame();
    }

    private void StartGame()
    {
        // Instanciar los jugadores
        //PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoint_Player1.transform.position, Quaternion.identity);
        //PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoint_Player2.transform.position, Quaternion.identity);
    }

    void CreateNewRoom()
    {
        string roomName = "Room " + Random.Range(1000, 10000);
        PhotonNetwork.CreateRoom(roomName, new Photon.Realtime.RoomOptions { MaxPlayers = MAX_PLAYERS_PER_ROOM });
    }
}
