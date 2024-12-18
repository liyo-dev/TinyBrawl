using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class LabyrinthGenerator : MonoBehaviourPun
{
    public GameObject wallPrefab;       // Prefab de las paredes
    public GameObject floorPrefab;      // Prefab del suelo
    public GameObject finalPrefab;      // Prefab de la gema final

    public int mazeSize = 11;           // Tamaño del laberinto (debe ser impar para garantizar un centro)
    public int roomCount = 10;          // Número de jugadores y salas

    public int roomSize = 8;            // Tamaño de cada sala
    public int corridorSize = 3;        // Tamaño de los pasillos

    public PlayerSpawner playerSpawner;

    private Vector3 centerPosition;     // Posición del centro del laberinto
    private List<Vector3> spawnPositions = new List<Vector3>();

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateMaze();
            CalculateSpawnPositions();
            photonView.RPC(nameof(SyncMaze), RpcTarget.Others);
            photonView.RPC(nameof(SpawnPlayers), RpcTarget.All);
        }
    }

    #region Maze Generation
    void GenerateMaze()
    {
        int totalWidth = mazeSize * (roomSize + corridorSize);
        int totalHeight = mazeSize * (roomSize + corridorSize);

        // Generar suelos y paredes
        for (int x = 0; x < mazeSize; x++)
        {
            for (int z = 0; z < mazeSize; z++)
            {
                // Calcular posición de la sala
                Vector3 roomPosition = new Vector3(x * (roomSize + corridorSize), 0, z * (roomSize + corridorSize));
                GenerateRoom(roomPosition);

                // Determinar si estamos en el centro
                if (x == mazeSize / 2 && z == mazeSize / 2)
                {
                    centerPosition = roomPosition;

                    // Instanciar la gema localmente en el MasterClient
                    Vector3 gemPosition = centerPosition + new Vector3(0, 1f, 0); // Añadir altura
                   // GameObject gem = Instantiate(finalPrefab, gemPosition, Quaternion.identity);

                    // Notificar a los demás jugadores la posición de la gema
                    photonView.RPC(nameof(SyncFinalPosition), RpcTarget.All, gemPosition);
                }
            }
        }

        // Colocar paredes exteriores
        GenerateBorderWalls(totalWidth, totalHeight);
    }


    void GenerateRoom(Vector3 position)
    {
        // Generar suelo
        Instantiate(floorPrefab, position, Quaternion.identity, this.transform);

        // Generar paredes alrededor de la sala
        float halfRoom = roomSize / 2f;

        Instantiate(wallPrefab, position + new Vector3(-halfRoom, 0, 0), Quaternion.Euler(0, 90, 0), this.transform); // Izquierda
        Instantiate(wallPrefab, position + new Vector3(halfRoom, 0, 0), Quaternion.Euler(0, 90, 0), this.transform);  // Derecha
        Instantiate(wallPrefab, position + new Vector3(0, 0, halfRoom), Quaternion.identity, this.transform);       // Arriba
        Instantiate(wallPrefab, position + new Vector3(0, 0, -halfRoom), Quaternion.identity, this.transform);      // Abajo
    }

    void GenerateBorderWalls(int width, int height)
    {
        float halfWall = 0.5f;

        // Generar paredes horizontales
        for (float x = -halfWall; x <= width + halfWall; x += 1f)
        {
            Instantiate(wallPrefab, new Vector3(x, 0, -halfWall), Quaternion.identity, this.transform);
            Instantiate(wallPrefab, new Vector3(x, 0, height + halfWall), Quaternion.identity, this.transform);
        }

        // Generar paredes verticales
        for (float z = -halfWall; z <= height + halfWall; z += 1f)
        {
            Instantiate(wallPrefab, new Vector3(-halfWall, 0, z), Quaternion.Euler(0, 90, 0), this.transform);
            Instantiate(wallPrefab, new Vector3(width + halfWall, 0, z), Quaternion.Euler(0, 90, 0), this.transform);
        }
    }

    #endregion

    #region Player Spawn Points
    void CalculateSpawnPositions()
    {
        spawnPositions.Clear();

        // Generar posiciones alrededor del centro equidistantes
        int radius = mazeSize / 2;
        for (int i = 0; i < roomCount; i++)
        {
            float angle = i * (360f / roomCount); // Distribuir en círculo
            float x = centerPosition.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad) * (roomSize + corridorSize);
            float z = centerPosition.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad) * (roomSize + corridorSize);
            spawnPositions.Add(new Vector3(x, 0, z));
        }
    }
    #endregion

    #region Sync and Player Spawn
    [PunRPC]
    void SyncMaze()
    {
        // Si no es maestro, ejecuta el mismo generador localmente
        GenerateMaze();
        CalculateSpawnPositions();
    }

    [PunRPC]
    void SpawnPlayers()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        if (playerIndex >= 0 && playerIndex < spawnPositions.Count)
        {
            Vector3 spawnPosition = spawnPositions[playerIndex];
            spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y + 2, spawnPosition.z);
            //playerSpawner.MovePlayerToPosition(spawnPosition);
            playerSpawner.SpawnPoint.position = spawnPosition;
        }
        else
        {
            Debug.LogError("Spawn position not found for player.");
        }
    }

    [PunRPC]
    void SyncFinalPosition(Vector3 position)
    {
        // Instanciar la gema en los clientes no maestros
        Instantiate(finalPrefab, position, Quaternion.identity);
    }
    #endregion
}