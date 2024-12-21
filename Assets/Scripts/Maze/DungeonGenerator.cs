using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DungeonGenerator : MonoBehaviourPun
{
    [Header("Room Settings")]
    public GameObject[] roomPrefabs; // Prefabs de salas estándar
    public GameObject bossRoomPrefab; // Prefab para la sala del jefe
    public GameObject startRoomPrefab; // Prefab para las salas iniciales de los jugadores

    [Header("Dungeon Settings")]
    public int numRooms = 20; // Número total de salas (sin incluir salas iniciales)
    public float roomSpacing = 15f; // Distancia entre las salas

    private List<Vector3> roomPositions = new List<Vector3>(); // Posiciones generadas de las salas
    private List<int> roomTypes = new List<int>(); // Tipos de salas generadas

    void Start()
    {
       /* if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncDungeon", RpcTarget.Others, roomTypes.ToArray(), roomPositions.ToArray());
        }*/
            GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // Generar salas iniciales para cada jugador
        int playerCount = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < playerCount; i++)
        {
            Vector3 startPosition = GetRandomPosition();
            CreateRoom(startRoomPrefab, startPosition);
        }

        // Generar salas intermedias
        for (int i = 0; i < numRooms - 1; i++)
        {
            Vector3 position = GetRandomPosition();
            CreateRoom(roomPrefabs[Random.Range(0, roomPrefabs.Length)], position);
        }

        // Generar sala del jefe final
        Vector3 bossPosition = GetRandomPosition();
        CreateRoom(bossRoomPrefab, bossPosition);
    }

    void CreateRoom(GameObject roomPrefab, Vector3 position)
    {
        if (!roomPositions.Contains(position))
        {
            Instantiate(roomPrefab, position, Quaternion.identity);
            roomPositions.Add(position);
            roomTypes.Add(GetRoomTypeIndex(roomPrefab));
        }
    }

    int GetRoomTypeIndex(GameObject roomPrefab)
    {
        if (roomPrefab == bossRoomPrefab)
            return -1; // Índice especial para la sala del jefe
        else if (roomPrefab == startRoomPrefab)
            return -2; // Índice especial para salas iniciales
        return System.Array.IndexOf(roomPrefabs, roomPrefab);
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(-10, 10) * roomSpacing,
            0,
            Random.Range(-10, 10) * roomSpacing
        );
    }

    [PunRPC]
    void SyncDungeon(int[] syncedRoomTypes, Vector3[] syncedPositions)
    {
        for (int i = 0; i < syncedRoomTypes.Length; i++)
        {
            GameObject roomPrefab;
            if (syncedRoomTypes[i] == -1)
                roomPrefab = bossRoomPrefab;
            else if (syncedRoomTypes[i] == -2)
                roomPrefab = startRoomPrefab;
            else
                roomPrefab = roomPrefabs[syncedRoomTypes[i]];

            Instantiate(roomPrefab, syncedPositions[i], Quaternion.identity);
        }
    }
}
