using System;
using Photon.Pun;
using Photon.Realtime;
using Service;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyLauncherOnline : MonoBehaviourPunCallbacks
{
    private int maxPlayersPerRoom = GameConfig.MAX_PLAYERS;
    public TMP_InputField roomCodeInputField;
    public Button joinPublicButton;
    public Button joinPrivateButton;
    public Button roomCodeButton;
    public Button canContinue;
    public GameObject waiting;
    public GameObject noPlayersFound;
    private Timer timer;

    private bool isMasterPlayer = false;
    private bool shouldJoinRandomRoom = false;

    void Start()
    {
        timer = FindFirstObjectByType<Timer>();

        // Conectar a Photon solo si no está conectado
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        joinPublicButton.onClick.AddListener(() => {
            ResetUI(); // Resetear la UI antes de intentar unirse
            if (PhotonNetwork.IsConnectedAndReady)
            {
                JoinRandomRoomOrCreate();
            }
            else
            {
                shouldJoinRandomRoom = true;
                PhotonNetwork.ConnectUsingSettings(); // Reconectar
            }
        });

        joinPrivateButton.onClick.AddListener(() => {
            ResetUI(); // Resetear la UI antes de intentar unirse
            OnJoinPrivateButtonClicked();
        });

        roomCodeButton.onClick.AddListener(() => JoinPrivateRoom(roomCodeInputField.text));
        canContinue.onClick.AddListener(OnCanContinueClicked);

        // Desactivar botones hasta estar conectado al maestro
        joinPublicButton.interactable = false;
        joinPrivateButton.interactable = false;
        canContinue.interactable = false;

        // Suscribirse a los eventos del temporizador
        timer.OnTimeChanged += HandleTimerUpdate;
        timer.OnTimerCompleted += HandleTimerComplete;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al maestro de Photon.");
        // Activar botones una vez conectado al maestro
        joinPublicButton.interactable = true;
        joinPrivateButton.interactable = true;

        // Si la bandera está configurada, unirse a una sala aleatoria
        if (shouldJoinRandomRoom)
        {
            shouldJoinRandomRoom = false; // Restablecer la bandera
            JoinRandomRoomOrCreate();
        }
    }

    private void JoinRandomRoomOrCreate()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRandomRoom();
            waiting.SetActive(true);
            joinPublicButton.gameObject.SetActive(false);
            joinPrivateButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No se puede unir a una sala, el cliente no está listo.");
        }
    }

    private void OnJoinPrivateButtonClicked()
    {
        if (roomCodeInputField != null && roomCodeButton != null)
        {
            roomCodeInputField.gameObject.SetActive(true);
            roomCodeButton.gameObject.SetActive(true);
        }
    }

    private void JoinPrivateRoom(string roomCode)
    {
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogWarning("El código de la sala no puede estar vacío.");
            return;
        }

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayersPerRoom };
        PhotonNetwork.JoinOrCreateRoom(roomCode, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinRandomFailed(short returnCode, String message)
    {
        Debug.LogWarning("No se pudo unir a una sala aleatoria: " + message);
        CreateRoom();
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayersPerRoom };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Jugador unido a la sala. Jugadores actuales: " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            isMasterPlayer = true;
            ServiceLocator.GetService<LocalOnlineOption>().IsMasterClient = true;
            timer.StartTimer(); // Iniciar el temporizador cuando el primer jugador se une
        }
        else
        {
            timer.gameObject.SetActive(false); // Ocultar UI del temporizador para los jugadores no maestros
            canContinue.gameObject.SetActive(false);
        }

        CheckPlayersAndUpdateUI();
    }

    private void CheckPlayersAndUpdateUI()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        canContinue.interactable = playerCount >= 2;
        waiting.SetActive(playerCount < maxPlayersPerRoom);
        noPlayersFound.SetActive(false);
    }

    private void HandleTimerUpdate(float timeRemaining)
    {
        if (isMasterPlayer)
        {
            Debug.Log("Tiempo restante: " + timeRemaining * timer.duration + " segundos");
            // Actualizar UI del temporizador solo para el jugador maestro
            // Aquí puedes actualizar una UI específica del temporizador si tienes una.
        }
    }

    private void HandleTimerComplete()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            StartGame();
        }
        else
        {
            PhotonNetwork.LeaveRoom();
            noPlayersFound.SetActive(true);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Jugador entró a la sala. Jugadores actuales: " + PhotonNetwork.CurrentRoom.PlayerCount);
        CheckPlayersAndUpdateUI();

        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersPerRoom)
        {
            if (isMasterPlayer)
            {
                timer.ForceCompleteTimer(); // Forzar la finalización del temporizador si se alcanza el número máximo de jugadores
            }
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Error al crear la sala: " + message);
        CreateRoom();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Jugador ha salido de la sala. Jugadores actuales: " + PhotonNetwork.CurrentRoom.PlayerCount);
        CheckPlayersAndUpdateUI();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        ResetUI();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Desconectado de Photon: " + cause.ToString());
        ResetUI();
    }

    private void StartGame()
    {
        Debug.Log("Iniciando el juego...");
        photonView.RPC(nameof(CallGoSelectGameRPC), RpcTarget.All);
    }

    private void OnCanContinueClicked()
    {
        if (isMasterPlayer && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            StartGame();
        }
    }

    private void ResetUI()
    {
        Debug.Log("Reiniciando la UI...");

        // Reactivar botones de unión
        joinPublicButton.gameObject.SetActive(true);
        joinPrivateButton.gameObject.SetActive(true);

        // Restablecer la interactividad de los botones
        joinPublicButton.interactable = true;
        joinPrivateButton.interactable = true;
        canContinue.interactable = false;

        waiting.SetActive(false);

        // Ocultar el campo de entrada de código de sala y el botón
        if (roomCodeInputField != null && roomCodeButton != null)
        {
            roomCodeInputField.gameObject.SetActive(false);
            roomCodeButton.gameObject.SetActive(false);
        }

        isMasterPlayer = false;
    }

    [PunRPC]
    private void CallGoSelectGameRPC()
    {
        SceneManager.LoadScene(SceneNames.ChooseGame.ToString());
    }
}
