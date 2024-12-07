using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class MyGameManager : MonoBehaviourPunCallbacks
{
    private TextMeshProUGUI winnerText;
    private TextMeshProUGUI scoreTextLocal;
    private TextMeshProUGUI scoreTextRemote;
    private int score = 0;
    private CountdownTimer timer;

    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    public void Start()
    {
        Invoke(nameof(DoStart), 2f);
    }

    public void DoStart()
    {
        scoreTextLocal = GameObject.FindWithTag("ScoreLocal").GetComponent<TextMeshProUGUI>();
        winnerText = GameObject.FindWithTag("WinnerText").GetComponent<TextMeshProUGUI>();
        CountdownTimer timer = FindObjectOfType<CountdownTimer>();
        if (timer != null)
        {
            timer.OnTimeOut += DoStop;
        }
    }

    public void DoStop()
    {
        CalculateWinnerAndDisplay();
    }

    public void IncreaseLocalScore(int _score)
    {
        score += _score;

        if (scoreTextLocal != null)
        {
            scoreTextLocal.text = "Player " + PhotonNetwork.LocalPlayer.ActorNumber + ": " + score.ToString();
        }
        else
        {
            Debug.LogError("scoreTextLocal no ha sido inicializado correctamente.");
        }

        photonView.RPC("SyncScore", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber, score);
    }

    public void DecreaseLocalScore(int _score)
    {
        if (score == 0) return;

        score -= _score;

        if (scoreTextLocal != null)
        {
            scoreTextLocal.text = "Player " + PhotonNetwork.LocalPlayer.ActorNumber + ": " + score.ToString();
        }
        else
        {
            Debug.LogError("scoreTextLocal no ha sido inicializado correctamente.");
        }

        photonView.RPC("SyncScore", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber, score);
    }

    void CalculateWinnerAndDisplay()
    {
        // Lista de posibles nombres de scripts
        string[] scriptNames = { "GameImpostor", "GameBurguer", "GameFishing" };

        foreach (string scriptName in scriptNames)
        {
            // Intenta encontrar el script en la escena
            var script = FindObjectOfType(System.Type.GetType(scriptName));
            if (script != null)
            {
                // Usa reflexión para llamar al método "DoStop" si existe
                var method = script.GetType().GetMethod("DoStop");
                if (method != null)
                {
                    method.Invoke(script, null); // Llama al método sin parámetros
                    Debug.Log($"Llamado a DoStop en {scriptName}");
                }
            }
        }

        // Mostrar la puntuación
        winnerText.text = "Tu puntuación: " + score;

        // Obtener la puntuación del jugador local
        var myScore = score;

        //guardo los puntos en el SO del player y en playfab
        playerDataSO.points = myScore;

        // Guardar los puntos en PlayFab
        UpdatePointsInPlayFab(score);

        // Enviar la puntuación del jugador local al otro cliente
        photonView.RPC("ReceiveOpponentScore", RpcTarget.Others, myScore);
    }

    private void UpdatePointsInPlayFab(int newPoints)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Points", newPoints.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("Puntos actualizados en PlayFab correctamente.");
        }, error =>
        {
            Debug.LogError($"Error al actualizar puntos en PlayFab: {error.GenerateErrorReport()}");
        });
    }

    [PunRPC]
    private void SyncScore(int playerNumber, int newScore)
    {
        if (scoreTextRemote == null)
        {
            scoreTextRemote = GameObject.FindWithTag("ScoreRemote").GetComponent<TextMeshProUGUI>();
            if (scoreTextRemote == null)
            {
                Debug.LogError("No se encontró un objeto con la etiqueta 'ScoreRemote' o el componente TextMeshProUGUI en el objeto.");
                return;
            }
        }

        scoreTextRemote.text = "Player" + playerNumber + ": " + newScore;
    }

    [PunRPC]
    void ReceiveOpponentScore(int opponentScore)
    {
        // Obtener la puntuación del jugador local
        var myScore = score;

        if (myScore > opponentScore)
        {
            winnerText.text = "¡Has ganado!";
        }
        else if (myScore < opponentScore)
        {
            winnerText.text = "¡Has perdido!";
        }
        else
        {
            winnerText.text = "¡Empate!";
        }
    }

    private void OnDestroy()
    {
        CountdownTimer timer = FindObjectOfType<CountdownTimer>();

        if (timer != null)
        {
            timer.OnTimeOut -= DoStop;
        }
    }
}
