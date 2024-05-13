using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum MinigameType
{
    Impostor,
    FollowMe
}

public class MyGameManager : MonoBehaviourPunCallbacks
{
    public MinigameType SelectedMinigameType;
    private TextMeshProUGUI winnerText;
    private TextMeshProUGUI scoreTextLocal;
    private TextMeshProUGUI scoreTextRemote;
    private int score = 0;
    
    public void DoStart()
    {
        switch (SelectedMinigameType)
        {
            case MinigameType.Impostor:
                StartImpostor();
                break;
            case MinigameType.FollowMe:
                StartFollowMe();
                break;
        }
    }

    public void DoStop()
    {
        CalculateWinnerAndDisplay();
    }

    private void StartFollowMe()
    {
        // Implementa la lógica de inicio del minijuego FollowMe si es necesario
    }

    private void StartImpostor()
    {
        if (LocalOnlineOption.instance.IsOnline())
            FindObjectOfType<GameImpostor>().DoStart();
        else
            FindObjectOfType<GameImpostorLocal>().DoStart();
        
        scoreTextLocal = GameObject.FindWithTag("ScoreLocal").GetComponent<TextMeshProUGUI>();
        winnerText = GameObject.FindWithTag("WinnerText").GetComponent<TextMeshProUGUI>();
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

        if (LocalOnlineOption.instance.IsOnline())
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

        if (LocalOnlineOption.instance.IsOnline())
            photonView.RPC("SyncScore", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber, score);
    }

    void CalculateWinnerAndDisplay()
    {
        if (LocalOnlineOption.instance.IsOnline())
            FindObjectOfType<GameImpostor>().DoStop();
        else
        {
            FindObjectOfType<GameImpostorLocal>().DoStop();
            winnerText.text = "Tu puntuación: " + score;
        }
        
        // Obtener la puntuación del jugador local
        var myScore = score;

        if (LocalOnlineOption.instance.IsOnline())
            // Enviar la puntuación del jugador local al otro cliente
            photonView.RPC("ReceiveOpponentScore", RpcTarget.Others, myScore);
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
}
