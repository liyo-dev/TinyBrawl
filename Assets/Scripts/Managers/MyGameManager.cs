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

        score = 0;

        UpdateLocalScoreUI();
    }

    public void DoStop()
    {
        CalculateWinnerAndDisplay();
    }

    public void IncreaseLocalScore(int _score)
    {
        score += _score;

        UpdateLocalScoreUI();

        photonView.RPC("SyncScore", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber, score);
    }

    public void DecreaseLocalScore(int _score)
    {
        if (score == 0) return;

        score -= _score;

        UpdateLocalScoreUI();

        photonView.RPC("SyncScore", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber, score);
    }

    private void UpdateLocalScoreUI()
    {
        if (scoreTextLocal != null)
        {
            scoreTextLocal.text = "Player " + PhotonNetwork.LocalPlayer.ActorNumber + ": " + score.ToString();
        }
        else
        {
            Debug.LogError("scoreTextLocal no ha sido inicializado correctamente.");
        }
    }

    void CalculateWinnerAndDisplay()
    {
        // Lista de posibles nombres de scripts
        string[] scriptNames = { "GameImpostor", "GameBurguer", "GameFishing" };

        foreach (string scriptName in scriptNames)
        {
            var script = FindObjectOfType(System.Type.GetType(scriptName));
            if (script != null)
            {
                var method = script.GetType().GetMethod("DoStop");
                if (method != null)
                {
                    method.Invoke(script, null);
                    Debug.Log($"Llamado a DoStop en {scriptName}");
                }
            }
        }

        // Mostrar la puntuación
        winnerText.text = "Tu puntuación: " + score;

        // Actualizar los puntos en el SO
        playerDataSO.points += score;

        // Guardar los puntos en PlayFab
        UpdatePointsInPlayFab(playerDataSO.points);

        // Enviar la puntuación del jugador local al otro cliente
        photonView.RPC("ReceiveOpponentScore", RpcTarget.Others, score);
    }

    private void UpdatePointsInPlayFab(int totalPoints)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Points", totalPoints.ToString() }
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
