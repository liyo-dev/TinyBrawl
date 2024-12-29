using ExitGames.Client.Photon;
using Photon.Pun;
using Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomsPlacer : MonoBehaviourPunCallbacks
{
    public static RoomsPlacer Instance;

    public UnityEvent OnPlayerMoved;

    public PlayerSpawner PlayerSpawner;

    [Header("Prefabs")]
    public List<GameObject> RoomPrefabs;

    [Header("Size")]
    [Tooltip("How many rooms to create")]
    public int roomsAmount;
    [Tooltip("Room size in units")]
    public int roomSize = 12;

    [Header("Appearance")]
    public bool overrideMaterials;
    public Material[] Materials;

    [Header("Loot")]
    public bool overrideLoot;
    public GameObject[] LootPrefabs;

    [Header("Enemies")]
    public bool overrideEnemies;
    public GameObject[] EnemiesPrefabs;

    private Room[,] spawnedRooms;
    private bool lootRoomSpawned = false;
    private bool enemyRoomSpawned = false;
    private List<Vector3> roomPositions = new List<Vector3>();
    private List<int> roomTypes = new List<int>();

    private bool firstPlayerNotified = false;

    private void Start()
    {
        Instance = this;
    }

    private IEnumerator InitializeLabyrinth()
    {
        yield return new WaitForSeconds(2f);

        if (firstPlayerNotified)
        {
            GenerateLabyrinth();

            StartCoroutine(WaitBeforeRpc());
        }
        else
        {
            // Esperar hasta que el laberinto haya sido generado por el maestro
            while (roomPositions.Count == 0)
            {
                yield return null; // Esperar un frame
            }
        }
    }

    IEnumerator WaitBeforeRpc()
    {
        yield return new WaitForSeconds(1);
        photonView.RPC(nameof(SyncRoomPositions), RpcTarget.All, roomPositions.Select(pos => new float[] { pos.x, pos.y, pos.z }).ToArray());
        photonView.RPC(nameof(SpawnPlayer), RpcTarget.All);
    }

    private void GenerateLabyrinth()
    {
        spawnedRooms = new Room[roomsAmount * 10, roomsAmount * 10];

        var initialRoom = RoomPrefabs.Find(x => x.GetComponent<Room>().isStartRoom == true);
        if (initialRoom != null)
        {
            var roomInstance = PhotonNetwork.Instantiate(initialRoom.name, Vector3.zero, Quaternion.identity).GetComponent<Room>();
            roomInstance.gameObject.tag = "Room"; // Asegúrate de que la sala tenga el tag "Room"
            spawnedRooms[0, 0] = roomInstance;
            roomPositions.Add(roomInstance.transform.position);
            roomTypes.Add(RoomPrefabs.IndexOf(initialRoom));

            for (int i = 0; i < roomsAmount; i++)
                PlaceOneRoom();
        }
        else
        {
            Debug.LogWarning("SET AT LEAST ONE ROOM AS A START ROOM");
        }
    }

    private void PlaceOneRoom()
    {
        HashSet<Vector2Int> vacantPlaces = new HashSet<Vector2Int>();
        for (int x = 0; x < spawnedRooms.GetLength(0); x++)
        {
            for (int y = 0; y < spawnedRooms.GetLength(1); y++)
            {
                if (spawnedRooms[x, y] == null) continue;

                int maxX = spawnedRooms.GetLength(0) - 1;
                int maxY = spawnedRooms.GetLength(1) - 1;

                if (x > 0 && spawnedRooms[x - 1, y] == null)
                    vacantPlaces.Add(new Vector2Int(x - 1, y));
                if (y > 0 && spawnedRooms[x, y - 1] == null)
                    vacantPlaces.Add(new Vector2Int(x, y - 1));
                if (x < maxX && spawnedRooms[x + 1, y] == null)
                    vacantPlaces.Add(new Vector2Int(x + 1, y));
                if (y < maxY && spawnedRooms[x, y + 1] == null)
                    vacantPlaces.Add(new Vector2Int(x, y + 1));
            }
        }

        Room newRoom;

        if (!lootRoomSpawned)
        {
            newRoom = PhotonNetwork.Instantiate(RoomPrefabs.Find(x => x.GetComponent<Room>().isLootRoom == true).name, Vector3.zero, Quaternion.identity).GetComponent<Room>();
            lootRoomSpawned = true;
        }
        else if (!enemyRoomSpawned)
        {
            newRoom = PhotonNetwork.Instantiate(RoomPrefabs.Find(x => x.GetComponent<Room>().isEnemyRoom == true).name, Vector3.zero, Quaternion.identity).GetComponent<Room>();
            enemyRoomSpawned = true;
        }
        else
        {
            newRoom = PhotonNetwork.Instantiate(RoomPrefabs[UnityEngine.Random.Range(0, RoomPrefabs.Count)].name, Vector3.zero, Quaternion.identity).GetComponent<Room>();
        }

        newRoom.gameObject.tag = "Room"; // Asegúrate de que la sala tenga el tag "Room"

        int limit = 500;
        while (limit-- > 0)
        {
            Vector2Int position = vacantPlaces.ElementAt(UnityEngine.Random.Range(0, vacantPlaces.Count));

            if (ConnectToSomething(newRoom, position))
            {
                newRoom.transform.position = new Vector3(position.x, 0, position.y) * roomSize;
                spawnedRooms[position.x, position.y] = newRoom;
                roomPositions.Add(newRoom.transform.position);

                string prefabName = newRoom.gameObject.name.Replace("(Clone)", "").Trim();
                int prefabIndex = RoomPrefabs.FindIndex(p => p.name == prefabName);
                roomTypes.Add(prefabIndex);

                return;
            }
        }

        PhotonNetwork.Destroy(newRoom.gameObject);
    }

    private bool ConnectToSomething(Room room, Vector2Int p)
    {
        int maxX = spawnedRooms.GetLength(0) - 1;
        int maxY = spawnedRooms.GetLength(1) - 1;

        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (room.DoorU != null && p.y < maxY && spawnedRooms[p.x, p.y + 1]?.DoorD != null)
            neighbours.Add(Vector2Int.up);
        if (room.DoorD != null && p.y > 0 && spawnedRooms[p.x, p.y - 1]?.DoorU != null)
            neighbours.Add(Vector2Int.down);
        if (room.DoorR != null && p.x < maxX && spawnedRooms[p.x + 1, p.y]?.DoorL != null)
            neighbours.Add(Vector2Int.right);
        if (room.DoorL != null && p.x > 0 && spawnedRooms[p.x - 1, p.y]?.DoorR != null)
            neighbours.Add(Vector2Int.left);

        if (neighbours.Count == 0)
        {
            Debug.LogWarning("No valid neighbours found to connect the room.");
            return false;
        }

        Vector2Int selectedDirection = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
        Room selectedRoom = spawnedRooms[p.x + selectedDirection.x, p.y + selectedDirection.y];

        if (selectedRoom == null)
        {
            Debug.LogError("Selected room is null.");
            return false;
        }

        if (selectedDirection == Vector2Int.up)
        {
            if (room.DoorU != null && selectedRoom.DoorD != null)
            {
                room.DoorU.SetActive(false);
                selectedRoom.DoorD.SetActive(false);
            }
        }
        else if (selectedDirection == Vector2Int.down)
        {
            if (room.DoorD != null && selectedRoom.DoorU != null)
            {
                room.DoorD.SetActive(false);
                selectedRoom.DoorU.SetActive(false);
            }
        }
        else if (selectedDirection == Vector2Int.right)
        {
            if (room.DoorR != null && selectedRoom.DoorL != null)
            {
                room.DoorR.SetActive(false);
                selectedRoom.DoorL.SetActive(false);
            }
        }
        else if (selectedDirection == Vector2Int.left)
        {
            if (room.DoorL != null && selectedRoom.DoorR != null)
            {
                room.DoorL.SetActive(false);
                selectedRoom.DoorR.SetActive(false);
            }
        }

        StartCoroutine(DeactivateDoorsWithDelay(room, selectedRoom, selectedDirection));

        return true;
    }

    private IEnumerator DeactivateDoorsWithDelay(Room room, Room selectedRoom, Vector2Int selectedDirection)
    {
        yield return new WaitForSeconds(0.5f);

        if (selectedDirection == Vector2Int.up)
        {
            if (room.DoorU != null && selectedRoom.DoorD != null)
            {
                room.DoorU.SetActive(false);
                selectedRoom.DoorD.SetActive(false);
            }
        }
        else if (selectedDirection == Vector2Int.down)
        {
            if (room.DoorD != null && selectedRoom.DoorU != null)
            {
                room.DoorD.SetActive(false);
                selectedRoom.DoorU.SetActive(false);
            }
        }
        else if (selectedDirection == Vector2Int.right)
        {
            if (room.DoorR != null && selectedRoom.DoorL != null)
            {
                room.DoorR.SetActive(false);
                selectedRoom.DoorL.SetActive(false);
            }
        }
        else if (selectedDirection == Vector2Int.left)
        {
            if (room.DoorL != null && selectedRoom.DoorR != null)
            {
                room.DoorL.SetActive(false);
                selectedRoom.DoorR.SetActive(false);
            }
        }

        photonView.RPC(nameof(DeactivateDoorsRPC), RpcTarget.Others, room.transform.position, selectedRoom.transform.position, new int[] { selectedDirection.x, selectedDirection.y });
    }

    [PunRPC]
    private void DeactivateDoorsRPC(Vector3 roomPosition, Vector3 selectedRoomPosition, int[] selectedDirectionArray)
    {
        Vector2Int selectedDirection = new Vector2Int(selectedDirectionArray[0], selectedDirectionArray[1]);
        Room room = FindRoomByPosition(roomPosition);
        Room selectedRoom = FindRoomByPosition(selectedRoomPosition);

        if (room == null || selectedRoom == null)
        {
            Debug.LogError("Room or selected room not found.");
            return;
        }

        if (selectedDirection == Vector2Int.up)
        {
            if (room.DoorU != null && selectedRoom.DoorD != null)
            {
                room.DoorU.SetActive(false);
                selectedRoom.DoorD.SetActive(false);
            }
        }
        else if (selectedDirection == Vector2Int.down)
        {
            if (room.DoorD != null && selectedRoom.DoorU != null)
            {
                room.DoorD.SetActive(false);
                selectedRoom.DoorU.SetActive(false);
            }
        }
        else if (selectedDirection == Vector2Int.right)
        {
            if (room.DoorR != null && selectedRoom.DoorL != null)
            {
                room.DoorR.SetActive(false);
                selectedRoom.DoorL.SetActive(false);
            }
        }
        else if (selectedDirection == Vector2Int.left)
        {
            if (room.DoorL != null && selectedRoom.DoorR != null)
            {
                room.DoorL.SetActive(false);
                selectedRoom.DoorR.SetActive(false);
            }
        }
    }

    private Room FindRoomByPosition(Vector3 position)
    {
        foreach (Room room in FindObjectsOfType<Room>())
        {
            if (ArePositionsClose(room.transform.position, position))
            {
                return room;
            }
        }
        return null;
    }

    [PunRPC]
    private void SyncRoomPositions(float[][] positions)
    {
        roomPositions = positions.Select(pos => new Vector3(pos[0], pos[1], pos[2])).ToList();
    }

    [PunRPC]
    public void SpawnPlayer()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber;

        if (playerIndex >= 1 && playerIndex < roomPositions.Count)
        {
            Vector3 spawnPosition = roomPositions[playerIndex - 1];
            Debug.Log($"Spawn position found: {spawnPosition}");

            PlayerSpawner.SpawnPoint.position = spawnPosition + new Vector3(0, 1, 0);
            PlayerSpawner.SpawnPlayer();
        }
        else
        {
            Debug.LogError("Spawn position not found for player.");
        }

        OnPlayerMoved.Invoke();
    }

    private bool ArePositionsClose(Vector3 pos1, Vector3 pos2, float tolerance = 0.1f)
    {
        return Vector3.Distance(pos1, pos2) < tolerance;
    }

    public override void OnJoinedRoom()
    {
        if (!firstPlayerNotified && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            firstPlayerNotified = true;
            StartCoroutine(nameof(InitializeLabyrinth));
        }
    }
}
