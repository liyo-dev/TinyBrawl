using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEngine.Events;

public class WorldManager : MonoBehaviourPunCallbacks
{
    public UnityEvent OnPlayerLoaded;

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
        // Configurar el nickname del jugador basado en PlayerDataSO
        if (playerDataSO != null && !string.IsNullOrEmpty(playerDataSO.username))
        {
            PhotonNetwork.NickName = playerDataSO.username;
            Debug.Log($"Nickname del jugador configurado como: {PhotonNetwork.NickName}");
        }
        else
        {
            Debug.LogWarning("PlayerDataSO no est� asignado o el username est� vac�o. Configurando un nickname predeterminado.");
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999); // Nickname predeterminado
        }

        // Conectar a Photon si no est� conectado
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
            MaxPlayers = 20, // L�mite de jugadores en la sala
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
        // Validar que el SO est� configurado
        if (playerDataSO == null)
        {
            Debug.LogError("PlayerDataSO no est� asignado.");
            return;
        }

        // Validar que los prefabs de personajes est�n configurados
        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogError("No se han asignado prefabs de personajes.");
            return;
        }

        // Validar el �ndice del personaje seleccionado
        int characterId = playerDataSO.selectedCharacterId;
        if (characterId < 0 || characterId >= characterPrefabs.Length)
        {
            Debug.LogWarning($"�ndice de personaje inv�lido ({characterId}). Se usar� el primer personaje.");
            characterId = 0; // Usar el primer personaje como predeterminado
        }

        // Destruir el personaje activo si existe
        if (activeCharacter != null)
        {
            Destroy(activeCharacter);
        }

        // Validar si el prefab est� en Resources (necesario para Photon)
        string prefabName = characterPrefabs[characterId].name;
        if (Resources.Load(prefabName) == null)
        {
            Debug.LogError($"El prefab {prefabName} no est� en la carpeta Resources. Aseg�rate de que el prefab est� disponible.");
            return;
        }

        // Instanciar el personaje seleccionado con Photon
        activeCharacter = PhotonNetwork.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation);

        Debug.Log($"Personaje instanciado: {prefabName}");

        // Validar que la c�mara virtual est� asignada
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera no est� asignada.");
            return;
        }

        // Buscar al jugador una vez instanciado
        InvokeRepeating(nameof(FindAndFollowPlayer), 0.5f, 1f);
    }

    private void FindAndFollowPlayer()
    {
        // Busca al jugador local instanciado por Photon
        GameObject player = GameObject.FindWithTag("Player"); // Aseg�rate de que el jugador tenga el tag "Player"

        if (player != null)
        {
            // Configurar la c�mara virtual para que siga al jugador
            virtualCamera.Follow = player.transform;

            // Configuraci�n para mantener la c�mara fija desde arriba
            Transform cameraTransform = virtualCamera.transform;
            cameraTransform.position = new Vector3(cameraTransform.position.x, 10f, cameraTransform.position.z);
            cameraTransform.rotation = Quaternion.Euler(50f, 0f, 0f);

            Debug.Log($"Cinemachine ahora sigue a: {player.name}");

            OnPlayerLoaded?.Invoke();

            // Detener la repetici�n, ya que hemos encontrado al jugador
            CancelInvoke(nameof(FindAndFollowPlayer));
        }
        else
        {
            Debug.LogWarning("No se encontr� ning�n jugador con el tag 'Player'.");
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
