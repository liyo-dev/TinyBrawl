using Photon.Pun;
using TMPro;
using UnityEngine;

public class MyGameManager : MonoBehaviourPunCallbacks
{
    private TextMeshProUGUI winnerText;
    private TextMeshProUGUI scoreTextLocal;
    private TextMeshProUGUI scoreTextRemote;
    private int score = 0;
    
    public void DoStart()
    {
        switch (LocalOnlineOption.instance.SelectedMiniGame)
        {
            case Minigame.Impostor:
                StartImpostor();
                break;
            case Minigame.Burguer:
                StartBurguer();
                break;
        }
    }

    public void DoStop()
    {
        CalculateWinnerAndDisplay();
    }

    private void StartBurguer()
    {
        if (LocalOnlineOption.instance.IsOnline())
        {
            FindObjectOfType<GameBurguer>().DoStart();
        }
        scoreTextLocal = GameObject.FindWithTag("ScoreLocal").GetComponent<TextMeshProUGUI>();
        winnerText = GameObject.FindWithTag("WinnerText").GetComponent<TextMeshProUGUI>();
    }

    private void StartImpostor()
    {
        if (LocalOnlineOption.instance.IsOnline())
        {
            FindObjectOfType<GameImpostor>().DoStart();
        }
        else
        {
            FindObjectOfType<GameImpostorLocal>().DoStart();
        }
        
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
        {
            if (LocalOnlineOption.instance.SelectedMiniGame == Minigame.Impostor)
                FindObjectOfType<GameImpostor>().DoStop();
            else if (LocalOnlineOption.instance.SelectedMiniGame == Minigame.Burguer)   
                FindObjectOfType<GameBurguer>().DoStop();
        }
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
