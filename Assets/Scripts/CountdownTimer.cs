using Photon.Pun;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI startText; // Nuevo texto para la cuenta regresiva inicio;
    private float totalTime = 60f;
    private float timeLeft;
    private bool countdownStarted = false; // Variable para controlar si la cuenta regresiva ha comenzado
    private PlayerMovement playerMovement;

    void Start()
    {
        photonView.RPC("QuitWaitForOthers", RpcTarget.All);
        timeLeft = totalTime;
        startText.text = "3";
        Invoke("CountdownTwo", 1f); // Llamará al método CountdownTwo después de 1 segundo
    }

    void Update()
    {
        if (countdownStarted)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft < 0)
            {
                timeLeft = 0;
            }

            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void CountdownTwo()
    {
        startText.text = "2";
        Invoke("CountdownOne", 1f); // Llamará al método CountdownOne después de 1 segundo
    }

    void CountdownOne()
    {
        startText.text = "1";
        Invoke("StartCountdown", 1f); // Llamará al método StartCountdown después de 1 segundo
    }

    void StartCountdown()
    {
        startText.gameObject.SetActive(false); // Ocultar el texto de la cuenta regresiva inicial
        countdownStarted = true; // Iniciar el cronómetro principal
        photonView.RPC("StartGame", RpcTarget.Others);
        Invoke("CalculateWinnerAndDisplay", totalTime);
    }

    [PunRPC]
    void QuitWaitForOthers()
    {
        GameObject waitForOthers = GameObject.FindWithTag("WaitForOthersTxt");
        waitForOthers.SetActive(false);
    }

    [PunRPC]
    void StartGame()
    {
        // Obtener el jugador local
        GameObject localPlayer = GameObject.FindWithTag("Player");

        if (localPlayer != null)
        {
            // Obtener el componente PlayerMovement del jugador local
            playerMovement = localPlayer.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                playerMovement.StartGame(); // Llamará al método StartGame del PlayerMovement
            }
            else
            {
                Debug.LogError("PlayerMovement no encontrado en el jugador local.");
            }
        }
        else
        {
            Debug.LogError("Jugador local no encontrado.");
        }
    }
    
    void CalculateWinnerAndDisplay()
    {
        playerMovement.EndGame();
        
        // Obtener todos los jugadores en la sala
        PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();

        // Inicializar variables para rastrear al jugador con más puntos
        int maxScore = int.MinValue;
        PlayerMovement winningPlayer = null;

        // Iterar sobre todos los jugadores para encontrar al que tiene más puntos
        foreach (PlayerMovement player in allPlayers)
        {
            if (player.score > maxScore)
            {
                maxScore = player.score;
                winningPlayer = player;
            }
        }

        // Mostrar el texto con el número del jugador que ganó
        if (winningPlayer != null)
        {
            countdownText.text = "Player " + winningPlayer.photonView.Owner.ActorNumber.ToString() + " gana";
            startText.gameObject.SetActive(false);
        }
        else
        {
            countdownText.text = "No winner";
        }
    }

}