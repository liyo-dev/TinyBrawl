using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsPlacer : MonoBehaviourPun
{
    public static RoomsPlacer Instance;

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

    private void Start()
    {
        Instance = this;

        StartCoroutine(nameof(InitializeLabyrinth));
    }

    private IEnumerator InitializeLabyrinth()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateLabyrinth();
        }
        else
        {
            // Esperar hasta que el laberinto haya sido generado por el maestro
            while (roomPositions.Count == 0)
            {
                yield return null; // Esperar un frame
            }
        }

        if (photonView.IsMine)
        {
            SpawnPlayer();
        }
    }

    private void GenerateLabyrinth()
    {
        spawnedRooms = new Room[roomsAmount * 10, roomsAmount * 10];

        var initialRoom = RoomPrefabs.Find(x => x.GetComponent<Room>().isStartRoom == true);
        if (initialRoom != null)
        {
            var roomInstance = PhotonNetwork.Instantiate(initialRoom.name, Vector3.zero, Quaternion.identity).GetComponent<Room>();
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
            newRoom = PhotonNetwork.Instantiate(RoomPrefabs[Random.Range(0, RoomPrefabs.Count)].name, Vector3.zero, Quaternion.identity).GetComponent<Room>();
        }

        int limit = 500;
        while (limit-- > 0)
        {
            Vector2Int position = vacantPlaces.ElementAt(Random.Range(0, vacantPlaces.Count));

            if (ConnectToSomething(newRoom, position))
            {
                newRoom.transform.position = new Vector3(position.x, 0, position.y) * roomSize;
                spawnedRooms[position.x, position.y] = newRoom;
                roomPositions.Add(newRoom.transform.position);
                roomTypes.Add(RoomPrefabs.IndexOf(newRoom.gameObject));

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
            return false;

        Vector2Int selectedDirection = neighbours[Random.Range(0, neighbours.Count)];
        Room selectedRoom = spawnedRooms[p.x + selectedDirection.x, p.y + selectedDirection.y];

        if (selectedDirection == Vector2Int.up)
        {
            room.DoorU.SetActive(false);
            selectedRoom.DoorD.SetActive(false);
        }
        else if (selectedDirection == Vector2Int.down)
        {
            room.DoorD.SetActive(false);
            selectedRoom.DoorU.SetActive(false);
        }
        else if (selectedDirection == Vector2Int.right)
        {
            room.DoorR.SetActive(false);
            selectedRoom.DoorL.SetActive(false);
        }
        else if (selectedDirection == Vector2Int.left)
        {
            room.DoorL.SetActive(false);
            selectedRoom.DoorR.SetActive(false);
        }

        if (room.DoorU != null && selectedDirection != Vector2Int.up)
            room.DoorU.SetActive(true);

        if (room.DoorD != null && selectedDirection != Vector2Int.down)
            room.DoorD.SetActive(true);

        if (room.DoorR != null && selectedDirection != Vector2Int.right)
            room.DoorR.SetActive(true);

        if (room.DoorL != null && selectedDirection != Vector2Int.left)
            room.DoorL.SetActive(true);

        return true;
    }

    public void SpawnPlayer()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        if (playerIndex >= 0 && playerIndex < roomPositions.Count)
        {
            Vector3 spawnPosition = roomPositions[playerIndex];
            GameObject.FindWithTag("Player").transform.position = spawnPosition + new Vector3(0, 1, 0);
        }
        else
        {
            Debug.LogError("Spawn position not found for player.");
        }
    }
}
