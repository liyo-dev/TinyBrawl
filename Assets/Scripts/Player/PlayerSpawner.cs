using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEngine.Events;
using PlayFab;
using PlayFab.ClientModels;
using Service;
using System.Linq;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public UnityEvent OnPlayerLoaded;

    [Header("Player Data")]
    [SerializeField] private int maxPlayersInRoom;

    [Header("Player Data")]
    private PlayerDataSO playerDataSO;

    [Header("Characters")]
    private GameObject[] characterPrefabs;

    [Header("Inventory Items")]
    private GameObject[] inventoryItems;

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;

    public Transform SpawnPoint => spawnPoint;

    [Header("Cinemachine Virtual Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    private float cameraRotation = 15f;

    private GameObject activeCharacter;

    public string RoomName = "";

    public bool WaitBeforeSpawn = false;

    private bool isNicknameConfigured = false;

    void Start()
    {
        playerDataSO = ServiceLocator.GetService<PlayerDataService>().GetData();

        inventoryItems = playerDataSO.inventory?.inventoryItems.ToArray();

        characterPrefabs = ServiceLocator.GetService<CharacterDataService>().GetData().characters.ToArray();

        // Configurar o recuperar el nickname del jugador
        if (playerDataSO != null && !string.IsNullOrEmpty(playerDataSO.username))
        {
            PhotonNetwork.NickName = playerDataSO.username;
            if (!isNicknameConfigured)
            {
                Debug.Log($"Nickname del jugador configurado como: {PhotonNetwork.NickName}");
                isNicknameConfigured = true;
            }

            ConnectToPhoton();
        }
        else
        {
            Debug.Log("Recuperando datos del usuario desde PlayFab...");
            RetrievePlayerDataFromPlayFab();
        }
    }

    private void RetrievePlayerDataFromPlayFab()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
        {
            if (result.AccountInfo != null && !string.IsNullOrEmpty(result.AccountInfo.Username))
            {
                playerDataSO.username = result.AccountInfo.Username;
                PhotonNetwork.NickName = playerDataSO.username;
                if (!isNicknameConfigured)
                {
                    Debug.Log($"Nickname del jugador configurado como: {PhotonNetwork.NickName}");
                    isNicknameConfigured = true;
                }
                ConnectToPhoton();
            }
            else
            {
                Debug.LogError("No se pudo recuperar el username del usuario desde PlayFab.");
            }
        }, error =>
        {
            Debug.LogError($"Error al recuperar datos del usuario desde PlayFab: {error.GenerateErrorReport()}");
        });
    }

    private void ConnectToPhoton()
    {
        // Conectar a Photon si no está conectado
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            // Verificar si ya estás en una sala
            if (!PhotonNetwork.InRoom)
            {
                JoinRoom();
            }
            else
            {
                PhotonNetwork.LeaveRoom();
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        JoinRoom();
    }

    public override void OnJoinedLobby()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            JoinRoom();
        }
    }

    private void JoinRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayersInRoom,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        if (!WaitBeforeSpawn)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        SpawnCharacter();
        EquipWeapons();
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

        activeCharacter = PhotonNetwork.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation);

        // Validar que la cámara virtual esté asignada
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera no está asignada.");
            return;
        }

        // Buscar al jugador una vez instanciado
        InvokeRepeating(nameof(FindAndFollowPlayer), 0.5f, 1f);
    }

    private void EquipWeapons()
    {
        // Usar GameObject.FindWithTag para encontrar las zonas de las manos
        GameObject leftHandObject = GameObject.FindWithTag("EquipLeft");
        GameObject rightHandObject = GameObject.FindWithTag("EquipRight");

        // Obtener los transform de las zonas de las manos
        Transform leftHandZone = leftHandObject != null ? leftHandObject.transform : null;
        Transform rightHandZone = rightHandObject != null ? rightHandObject.transform : null;

        if (leftHandZone != null && playerDataSO.leftHandItemId >= 0 && playerDataSO.leftHandItemId < inventoryItems.Length)
        {
            InstantiateWeapon(inventoryItems[playerDataSO.leftHandItemId], leftHandZone);
        }

        if (rightHandZone != null && playerDataSO.rightHandItemId >= 0 && playerDataSO.rightHandItemId < inventoryItems.Length)
        {
            InstantiateWeapon(inventoryItems[playerDataSO.rightHandItemId], rightHandZone);
        }
    }

    private void InstantiateWeapon(GameObject weaponPrefab, Transform handZone)
    {
        if (handZone.childCount > 0)
        {
            Destroy(handZone.GetChild(0).gameObject);
        }

        GameObject weapon = Instantiate(weaponPrefab, handZone);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }

    private void FindAndFollowPlayer()
    {
        // Busca al jugador local instanciado por Photon
        GameObject player = GameObject.FindWithTag("Player"); // Asegúrate de que el jugador tenga el tag "Player"

        if (player != null)
        {
            // Configurar la cámara virtual para que siga al jugador
            virtualCamera.Follow = player.transform;

            // Configuración para mantener la cámara fija desde arriba
            Transform cameraTransform = virtualCamera.transform;
            cameraTransform.position = new Vector3(cameraTransform.position.x, 10f, cameraTransform.position.z);
            cameraTransform.rotation = Quaternion.Euler(cameraRotation, 0f, 0f);

            ConfigureVirtualCamera();

            OnPlayerLoaded?.Invoke();

            // Detener la repetición, ya que hemos encontrado al jugador
            CancelInvoke(nameof(FindAndFollowPlayer));
        }
        else
        {
            Debug.LogWarning("No se encontró ningún jugador con el tag 'Player'.");
        }
    }

    private void ConfigureVirtualCamera()
    {
        if (virtualCamera != null)
        {
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_FollowOffset = new Vector3(0, 3, -10); // Configurar el Follow Offset
            }
            else
            {
                Debug.LogWarning("No se encontró un componente CinemachineTransposer en la cámara virtual.");
            }
        }
    }

    public override void OnLeftRoom()
    {
        Debug.LogWarning($"Abandonando la sala");
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
