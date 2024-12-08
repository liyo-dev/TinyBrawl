using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class WorldManager : MonoBehaviourPunCallbacks
{
    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    [Header("Characters")]
    [SerializeField] private GameObject[] characterPrefabs; // Lista de prefabs de personajes

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint; // Punto donde instanciar el personaje

    [Header("Cinemachine Virtual Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private GameObject activeCharacter; // Personaje actualmente instanciado

    private const string RoomName = "World";

    void Start()
    {
        // Conectar a Photon si no está conectado
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            JoinWorldRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor de Photon.");
        JoinWorldRoom();
    }

    private void JoinWorldRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20, // Límite de jugadores en la sala
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Conectado a la sala '{RoomName}' con {PhotonNetwork.CurrentRoom.PlayerCount} jugadores.");
        SpawnCharacter();
    }

    private void SpawnCharacter()
    {
        // Validar que el SO esté configurado
        if (playerDataSO == null)
        {
            Debug.LogError("PlayerDataSO no está asignado.");
            return;
        }

        // Validar que los prefabs de personajes estén configurados
        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogError("No se han asignado prefabs de personajes.");
            return;
        }

        // Validar el índice del personaje seleccionado
        int characterId = playerDataSO.selectedCharacterId;
        if (characterId < 0 || characterId >= characterPrefabs.Length)
        {
            Debug.LogWarning($"Índice de personaje inválido ({characterId}). Se usará el primer personaje.");
            characterId = 0; // Usar el primer personaje como predeterminado
        }

        // Destruir el personaje activo si existe
        if (activeCharacter != null)
        {
            Destroy(activeCharacter);
        }

        // Validar si el prefab está en Resources (necesario para Photon)
        string prefabName = characterPrefabs[characterId].name;
        if (Resources.Load(prefabName) == null)
        {
            Debug.LogError($"El prefab {prefabName} no está en la carpeta Resources. Asegúrate de que el prefab esté disponible.");
            return;
        }

        // Instanciar el personaje seleccionado con Photon
        activeCharacter = PhotonNetwork.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation);

        Debug.Log($"Personaje instanciado: {prefabName}");

        // Validar que la cámara virtual esté asignada
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera no está asignada.");
            return;
        }

        // Buscar al jugador una vez instanciado
        InvokeRepeating(nameof(FindAndFollowPlayer), 0.5f, 1f);
    }

    private void FindAndFollowPlayer()
    {
        // Busca al jugador local instanciado por Photon
        GameObject player = GameObject.FindWithTag("Player"); // Asegúrate de que el jugador tenga el tag "Player"

        if (player != null)
        {
            // Configurar la cámara virtual para que siga al jugador
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;

            Debug.Log($"Cinemachine ahora sigue a: {player.name}");

            // Detener la repetición, ya que hemos encontrado al jugador
            CancelInvoke(nameof(FindAndFollowPlayer));
        }
        else
        {
            Debug.LogWarning("No se encontró ningún jugador con el tag 'Player'.");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Error al unirse a la sala '{RoomName}': {message}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Desconectado de Photon: {cause}");
    }
}
