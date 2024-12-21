using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class LabyrinthGenerator : MonoBehaviourPun
{
    public GameObject wallPrefab;       // Prefab de las paredes
    public GameObject floorPrefab;      // Prefab del suelo
    public GameObject finalPrefab;      // Prefab de la gema final

    public int mazeWidth = 11;          // Ancho del laberinto (debe ser impar para garantizar un centro)
    public int mazeHeight = 11;         // Alto del laberinto (debe ser impar para garantizar un centro)
    public int roomCount = 10;          // Número de jugadores y salas

    public PlayerSpawner playerSpawner;

    private Vector3 finalPosition;      // Posición del final del laberinto
    private List<Vector3> spawnPositions = new List<Vector3>();
    private bool[,] maze;               // Matriz del laberinto

    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateMaze();
            CalculateSpawnPositions();
            photonView.RPC(nameof(SyncMaze), RpcTarget.Others, ConvertMazeToList(maze), finalPosition);
            photonView.RPC(nameof(SpawnPlayers), RpcTarget.All);
        }
    }

    #region Maze Generation
    void GenerateMaze()
    {
        maze = new bool[mazeWidth, mazeHeight];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(1, 1);
        maze[current.x, current.y] = true;
        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Pop();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                stack.Push(current);
                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                maze[chosen.x, chosen.y] = true;
                maze[(current.x + chosen.x) / 2, (current.y + chosen.y) / 2] = true;
                stack.Push(chosen);
            }
        }

        // Generar suelos y paredes
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeHeight; z++)
            {
                Vector3 position = new Vector3(x, 0, z);
                if (maze[x, z])
                {
                    Instantiate(floorPrefab, position, Quaternion.identity, this.transform);
                }
                else
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, this.transform);
                }
            }
        }

        // Colocar el final del laberinto en una posición aleatoria
        finalPosition = new Vector3(Random.Range(1, mazeWidth - 1), 1f, Random.Range(1, mazeHeight - 1));
        Instantiate(finalPrefab, finalPosition, Quaternion.identity);
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (cell.x > 1 && !maze[cell.x - 2, cell.y]) neighbors.Add(new Vector2Int(cell.x - 2, cell.y));
        if (cell.x < mazeWidth - 2 && !maze[cell.x + 2, cell.y]) neighbors.Add(new Vector2Int(cell.x + 2, cell.y));
        if (cell.y > 1 && !maze[cell.x, cell.y - 2]) neighbors.Add(new Vector2Int(cell.x, cell.y - 2));
        if (cell.y < mazeHeight - 2 && !maze[cell.x, cell.y + 2]) neighbors.Add(new Vector2Int(cell.x, cell.y + 2));

        return neighbors;
    }
    #endregion

    #region Player Spawn Points
    void CalculateSpawnPositions()
    {
        spawnPositions.Clear();

        // Generar posiciones alrededor del final equidistantes
        int radius = Mathf.Min(mazeWidth, mazeHeight) / 2;
        for (int i = 0; i < roomCount; i++)
        {
            float angle = i * (360f / roomCount); // Distribuir en círculo
            float x = finalPosition.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = finalPosition.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            spawnPositions.Add(new Vector3(x, 0, z));
        }
    }
    #endregion

    #region Sync and Player Spawn
    [PunRPC]
    void SyncMaze(List<bool> mazeData, Vector3 finalPos)
    {
        maze = ConvertListToMaze(mazeData, mazeWidth, mazeHeight);
        finalPosition = finalPos;

        // Generar suelos y paredes
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeHeight; z++)
            {
                Vector3 position = new Vector3(x, 0, z);
                if (maze[x, z])
                {
                    Instantiate(floorPrefab, position, Quaternion.identity, this.transform);
                }
                else
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, this.transform);
                }
            }
        }

        // Instanciar la gema en los clientes no maestros
        Instantiate(finalPrefab, finalPosition, Quaternion.identity);
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
            playerSpawner.SpawnPoint.position = spawnPosition;
        }
        else
        {
            Debug.LogError("Spawn position not found for player.");
        }
    }

    List<bool> ConvertMazeToList(bool[,] maze)
    {
        List<bool> mazeList = new List<bool>();
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int z = 0; z < maze.GetLength(1); z++)
            {
                mazeList.Add(maze[x, z]);
            }
        }
        return mazeList;
    }

    bool[,] ConvertListToMaze(List<bool> mazeList, int width, int height)
    {
        bool[,] maze = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                maze[x, z] = mazeList[x * height + z];
            }
        }
        return maze;
    }
    #endregion
}
