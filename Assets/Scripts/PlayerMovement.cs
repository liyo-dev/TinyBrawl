using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public float veloc = 5f;
    public int score = 0; // Puntuación del jugador

    private GameObject scorePrefab;

    private TextMeshProUGUI scoreText; // Referencia al texto en la UI para mostrar la puntuación del player local

    private bool isTouchingCast;
    
    void Start()
    {
        // Si este jugador es el local, configurar la referencia al texto de la puntuación
        if (photonView.IsMine)
        {
            scorePrefab = GameObject.FindWithTag("Score");
            
            if (scorePrefab != null)
            {
                var scoreList = scorePrefab.GetComponentsInChildren<TextMeshProUGUI>();
                scoreText = scoreList[photonView.Owner.ActorNumber - 1];
            }
            else
            {
                Debug.LogError("No se encontró el componente Canvas en el prefab ScoreObject.");
            }
        }
    }

    void Update()
    {
        // Si este jugador es el local, controlar su movimiento y puntuación
        if (photonView.IsMine)
        {
            // Obtener la posición del ratón en el mundo
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Asegurarse de que la coordenada Z sea cero en un juego 2D

            // Calcular la dirección del movimiento
            Vector2 direction = (mousePos - transform.position).normalized;

            // Mover el jugador en la dirección calculada
            transform.position += (Vector3)direction * veloc * Time.deltaTime;

            // Si se hace clic izquierdo, aumenta la puntuación y sincronízala con los demás jugadores
            if (Input.GetMouseButtonDown(0) && isTouchingCast)
            {
                IncreaseScore(1);
                photonView.RPC("SyncScore", RpcTarget.Others, photonView.Owner.ActorNumber, score);
            }
            else if (Input.GetMouseButtonDown(0) && !isTouchingCast)
            {
                DecreaseScore(1);
                photonView.RPC("SyncScore", RpcTarget.Others, photonView.Owner.ActorNumber, score);
            }
        }
    }

    // Método RPC para sincronizar la puntuación con todos los jugadores
    [PunRPC]
    void SyncScore(int playerActorNumber, int newScore)
    {
        GameObject scorePrefabRemote = GameObject.FindWithTag("Score");
            
        if (scorePrefabRemote != null)
        {
            var scoreList = scorePrefabRemote.GetComponentsInChildren<TextMeshProUGUI>();
            var scoreText = scoreList[playerActorNumber - 1];
            scoreText.text = "Player" + photonView.Owner.ActorNumber + ": " + newScore;
        }
    }


    // Método para aumentar la puntuación del jugador local
    void IncreaseScore(int amount)
    {
        // Aumentar la puntuación
        score += amount;

        // Mostrar la puntuación actualizada del jugador local en su ScoreObject
        if (scoreText != null)
        {
            scoreText.text = "Player" + photonView.Owner.ActorNumber + ": " + score;
            Debug.Log("Aumento puntuacion local: " + score);
        }
    }
    
    void DecreaseScore(int amount)
    {
        // Aumentar la puntuación
        score -= amount;

        // Mostrar la puntuación actualizada del jugador local en su ScoreObject
        if (scoreText != null)
        {
            scoreText.text = "Player" + photonView.Owner.ActorNumber + ": " + score;
            Debug.Log("Disminuyo puntuacion local: " + score);
        }
    }
    
    // Triggers
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cast"))
        {
            isTouchingCast = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cast"))
        {
            isTouchingCast = false;
        }
    }
}
