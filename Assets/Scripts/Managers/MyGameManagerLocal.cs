using TMPro;
using UnityEngine;

public class MyGameManagerLocal : MonoBehaviour
{
    private TextMeshProUGUI winnerText;
    private TextMeshProUGUI scoreTextLocal;
    private int score = 0;
    private CountdownTimer timer;

    public void Start()
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
            scoreTextLocal.text = "Player 1: " + score.ToString();
        }
        else
        {
            Debug.LogError("scoreTextLocal no ha sido inicializado correctamente.");
        }
    }

    public void DecreaseLocalScore(int _score)
    {
        if (score == 0) return;

        score -= _score;

        if (scoreTextLocal != null)
        {
            scoreTextLocal.text = "Player 1: " + score.ToString();
        }
        else
        {
            Debug.LogError("scoreTextLocal no ha sido inicializado correctamente.");
        }
    }

    void CalculateWinnerAndDisplay()
    {
        // Lista de posibles nombres de scripts
        string[] scriptNames = { "GameImpostorLocal", "GameBurguerLocal" };

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
