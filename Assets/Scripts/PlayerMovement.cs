using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public GameObject PowerUpVisual;
    public Transform PowerUpPosition;
    public float comboTimeThreshold = 1f;
    private int comboCount = 0;
    private float lastComboTime;
    public bool isPowerUpActive = false;
    private float lastPowerUpTime;
    public float powerUpCooldown = 5f;

    public float veloc = 1f;
    public int score = 0;

    private GameObject scorePrefab;
    private TextMeshProUGUI scoreText;
    private Image energy;

    private bool isTouchingCast;
    private bool isTouchingPlayer;
    
    private bool gameStarted = false;

    private void Start()
    {
        if (photonView.IsMine)
        {
            scorePrefab = GameObject.FindWithTag("Score");
            energy = GameObject.FindWithTag("Energy").GetComponent<Image>();

            if (scorePrefab != null)
            {
                var scoreList = scorePrefab.GetComponentsInChildren<TextMeshProUGUI>();
                scoreText = scoreList[photonView.Owner.ActorNumber - 1];
            }
            else
            {
                Debug.LogError("No se encontró el componente Canvas en el prefab ScoreObject.");
            }

            lastComboTime = Time.time;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (gameStarted) 
            {
                HandleMovement();
                HandleScoreInput();
                HandlePowerUpInput();
            }
        }
    }

    private void HandleMovement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (Vector3.Distance(transform.position, mousePos) > 0.1f)
        {
            Vector2 direction = (mousePos - transform.position).normalized;
            transform.position += (Vector3)direction * veloc * Time.deltaTime;
        }

        if (!isTouchingPlayer && !isTouchingCast && Input.GetMouseButtonDown(1) && veloc < 10)
        {
            veloc++;
        }

        // Gradually decrease velocity over time
        float decreaseRate = 0.5f; // Adjust this value as needed
        veloc -= decreaseRate * Time.deltaTime;

        // Ensure velocity doesn't go below zero
        veloc = Mathf.Max(0, veloc);

        energy.fillAmount = veloc / 10.0f;
    }


    private void HandleScoreInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTouchingCast)
            {
                IncreaseScore(1);
                photonView.RPC("SyncScore", RpcTarget.Others, photonView.Owner.ActorNumber, score);
            }
            else
            {
                DecreaseScore(1);
                photonView.RPC("SyncScore", RpcTarget.Others, photonView.Owner.ActorNumber, score);
            }
        }
    }

    private void HandlePowerUpInput()
    {
        if (Input.GetMouseButtonDown(1) && isPowerUpActive && Time.time - lastPowerUpTime >= powerUpCooldown)
        {
            LaunchPowerUp();
        }
    }

    private void LaunchPowerUp()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject powerUpInstance = PhotonNetwork.Instantiate(PowerUpVisual.name, PowerUpPosition.transform.position, Quaternion.identity);
        powerUpInstance.transform.DOMove(mousePos, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            PhotonNetwork.Destroy(powerUpInstance);
            isPowerUpActive = false;
            lastPowerUpTime = Time.time;
        });
    }

    [PunRPC]
    private void SyncScore(int playerActorNumber, int newScore)
    {
        GameObject scorePrefabRemote = GameObject.FindWithTag("Score");

        if (scorePrefabRemote != null)
        {
            var scoreList = scorePrefabRemote.GetComponentsInChildren<TextMeshProUGUI>();
            var scoreText = scoreList[playerActorNumber - 1];
            scoreText.text = "Player" + photonView.Owner.ActorNumber + ": " + newScore;
        }
    }
    
    [PunRPC]
    private void UpdateScore(int newScore)
    {
        score = newScore;
        photonView.RPC("SyncScore", RpcTarget.All, photonView.Owner.ActorNumber, score);
    }

    private void IncreaseScore(int amount)
    {
        if (Time.time - lastComboTime <= comboTimeThreshold)
        {
            comboCount++;
            if (comboCount >= 10)
            {
                isPowerUpActive = true;
                comboCount = 0;
            }
        }
        else
        {
            comboCount = 1;
        }

        lastComboTime = Time.time;
        score += amount;

        if (scoreText != null)
        {
            scoreText.text = "Player" + photonView.Owner.ActorNumber + ": " + score;
        }
    }
    
    private void DecreaseScore(int amount)
    {
        score -= amount;

        if (scoreText != null)
        {
            scoreText.text = "Player" + photonView.Owner.ActorNumber + ": " + score;
        }
    }
    
    // Método para iniciar el juego
    public void StartGame()
    {
        gameStarted = true;
    }
    
    public void EndGame()
    {
        gameStarted = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cast"))
        {
            isTouchingCast = true;
        }
        else if (other.CompareTag("PowerUp"))
        {
            photonView.RPC("UpdateScore", RpcTarget.Others, 0);
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
