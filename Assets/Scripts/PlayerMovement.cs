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

    //private GameObject scorePrefab;
    private TextMeshProUGUI scoreTextLocal;
    private TextMeshProUGUI scoreTextRemote;
    private Image energy;

    private bool isTouchingFollowMe;
    private bool isTouchingPlayer;
    
    private bool gameStarted = false;

    private void Start()
    {
        if (photonView.IsMine)
        {
            scoreTextLocal = GameObject.FindWithTag("ScoreLocal").GetComponent<TextMeshProUGUI>();
            energy = GameObject.FindWithTag("Energy").GetComponent<Image>();
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
        if (Input.touchSupported && Input.touchCount > 0)
        {
            // Si se detecta entrada táctil, mueve al personaje hacia la posición del toque
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchPos.z = 0;

            if (Vector3.Distance(transform.position, touchPos) > 0.1f)
            {
                Vector2 direction = (touchPos - transform.position).normalized;
                transform.position += (Vector3)direction * veloc * Time.deltaTime;
            }
        }
        else
        {
            // Si no se detecta entrada táctil, sigue al ratón
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (Vector3.Distance(transform.position, mousePos) > 0.1f)
            {
                Vector2 direction = (mousePos - transform.position).normalized;
                transform.position += (Vector3)direction * veloc * Time.deltaTime;
            }
        }
    
        float decreaseRate = 0.5f; 
        veloc -= decreaseRate * Time.deltaTime;
    
        veloc = Mathf.Max(1, veloc);

        energy.fillAmount = veloc / 10.0f;
    }



    private void HandleScoreInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTouchingFollowMe)
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
        if (scoreTextRemote == null)
        {
            scoreTextRemote = GameObject.FindWithTag("ScoreRemote").GetComponent<TextMeshProUGUI>();
        }
        
        scoreTextRemote.text = "Player" + photonView.Owner.ActorNumber + ": " + newScore;
    }
    
    [PunRPC]
    private void UpdateScore(int newScore)
    {
        score = newScore;
        photonView.RPC("SyncScore", RpcTarget.All, photonView.Owner.ActorNumber, score);
    }
    
    [PunRPC]
    private void DestroyVelocityPowerUp(GameObject _gameObject)
    {
        Destroy(_gameObject);
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

        if (scoreTextLocal != null)
        {
            scoreTextLocal.text = "Player" + photonView.Owner.ActorNumber + ": " + score;
        }
    }
    
    private void DecreaseScore(int amount)
    {
        score -= amount;

        if (scoreTextLocal != null)
        {
            scoreTextLocal.text = "Player" + photonView.Owner.ActorNumber + ": " + score;
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
        if (other.CompareTag("FollowMe"))
        {
            isTouchingFollowMe = true;
        }
        else if (other.CompareTag("PowerUp"))
        {
            photonView.RPC("UpdateScore", RpcTarget.All, 0);
        }
        else if (other.CompareTag("VelocityPower"))
        {
            if (!isTouchingPlayer && !isTouchingFollowMe && veloc < 10)
            {
                veloc += 2;
                //Destroy(other.gameObject);
                //photonView.RPC("DestroyVelocityPowerUp", RpcTarget.Others, other.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("FollowMe"))
        {
            isTouchingFollowMe = false;
        }
    }
}
