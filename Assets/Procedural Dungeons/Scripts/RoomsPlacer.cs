using Photon.Pun;
using Service;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public class RoomsPlacer : MonoBehaviourPunCallbacks
{
    public static RoomsPlacer Instance;

    public UnityEvent OnPlayerMoved;

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

        if (firstPlayerNotified)
        {
            Debug.Log("Llamo al RPC HolaTest...");
            photonView.RPC(nameof(HolaTest), RpcTarget.Others);
        }
        else
        {
            Debug.Log("No llamo al RPC.");
        }
    }

    private IEnumerator InitializeLabyrinth()
    {
        if (firstPlayerNotified)
        {
            GenerateLabyrinth();

            if (!photonView.IsMine)
            {
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                Debug.Log("Transferencia de propiedad realizada.");
            }

            if (photonView.IsMine)
            {
                StartCoroutine(WaitBeforeRpc());
            }
        }
        else
        {
            Debug.Log("No soy el maestro.");
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
        OnPlayerMoved.Invoke();
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

    [PunRPC]
    private void SyncRoomPositions(float[][] positions)
    {
        Debug.Log("Syncing room positions.");
        roomPositions = positions.Select(pos => new Vector3(pos[0], pos[1], pos[2])).ToList();
    }

    [PunRPC]
    public void SpawnPlayer()
    {
        Debug.Log("Spawning player.");
        StartCoroutine(SpawnPlayerWithRetries());
    }

    [PunRPC]
    public void HolaTest()
    {
        Debug.Log($"HolaTest llamado por: {PhotonNetwork.LocalPlayer.NickName}");
        Debug.Log($"PhotonView ID: {photonView.ViewID}");
    }



    private IEnumerator SpawnPlayerWithRetries(int maxRetries = 5, float retryDelay = 1.0f)
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber;

        Debug.Log($"Spawning player {playerIndex}.");

        Debug.Log($"Room positions: {string.Join(", ", roomPositions.Select(x => x.ToString()).ToArray())}");

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            if (playerIndex >= 1 && playerIndex < roomPositions.Count)
            {
                Vector3 spawnPosition = roomPositions[playerIndex - 1];

                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                GameObject[] characterPrefabs = ServiceLocator.GetService<CharacterDataService>().GetData().characters.ToArray();
                int characterId = ServiceLocator.GetService<PlayerDataService>().GetData().selectedCharacterId;
                string prefabName = characterPrefabs[characterId].name;

                foreach (GameObject player in players)
                {
                    if (player.name.StartsWith(prefabName))
                    {
                        player.transform.position = spawnPosition + new Vector3(0, 1, 0);
                        yield break;
                    }
                }

                Debug.Log($"Player prefab {prefabName} not found in the scene. Attempt {attempt + 1} of {maxRetries}.");
            }
            else
            {
                Debug.LogError("Spawn position not found for player.");
                yield break;
            }

            yield return new WaitForSeconds(retryDelay);
        }

        Debug.LogError("Failed to spawn player after multiple attempts.");
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
            StartCoroutine(nameof(InitializeLabyrinth));
        }
    }

}
