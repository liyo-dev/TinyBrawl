using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerDataSO;

    [Header("Health Settings")]
    public float maxHealth = 100f; // Vida máxima del jugador
    private float currentHealth;

    [Header("Health Bar")]
    private PlayerHealthBar healthBar;

    // HashSet para evitar múltiples procesamientos de la misma bala
    private HashSet<int> processedBullets = new HashSet<int>();

    private void Awake()
    {
        healthBar = GetComponentInChildren<PlayerHealthBar>();
        if (healthBar == null)
        {
            Debug.LogError("No se encontró el componente PlayerHealthBar como hijo del jugador.");
        }
    }

    private void Start()
    {
        if (playerDataSO != null)
        {
            maxHealth = playerDataSO.hp > 0 ? playerDataSO.hp : maxHealth;
            currentHealth = playerDataSO.hp > 0 ? playerDataSO.hp : maxHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.RestoreHealth(currentHealth);
        }

        // Sincronizar la salud inicial al conectar
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(SyncHealHealth), RpcTarget.AllBuffered, currentHealth);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Verificar si colisiona con una bala
        if (other.CompareTag("Bullet"))
        {
            PhotonView bulletPhotonView = other.GetComponent<PhotonView>();
            if (bulletPhotonView == null) return;

            // Evitar que la bala se procese varias veces
            if (processedBullets.Contains(bulletPhotonView.ViewID))
            {
                Debug.Log("Esta bala ya fue procesada.");
                return;
            }
            processedBullets.Add(bulletPhotonView.ViewID);

            // Evitar que el jugador se dañe a sí mismo
            if (bulletPhotonView.Owner == photonView.Owner)
            {
                Debug.Log("La bala pertenece al mismo jugador. Ignorando colisión.");
                return;
            }

            TakeDamage(10f);

            // Notificar al propietario de la bala para que la destruya
            if (bulletPhotonView.Owner != null)
            {
                photonView.RPC(nameof(DestroyBullet), bulletPhotonView.Owner, bulletPhotonView.ViewID);
            }
        }
    }

    [PunRPC]
    public void DestroyBullet(int bulletViewID)
    {
        PhotonView bulletView = PhotonNetwork.GetPhotonView(bulletViewID);
        if (bulletView != null && bulletView.IsMine)
        {
            PhotonNetwork.Destroy(bulletView.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        // Sincronizar la salud con todos los jugadores (incluso los que entren después)
        photonView.RPC(nameof(SyncReduceHealth), RpcTarget.AllBuffered, currentHealth);

        // Verificar si el jugador murió
        if (currentHealth <= 0)
        {
            Die();
        }
    }




    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Actualizar la barra de vida
        if (healthBar != null)
        {
            healthBar.RestoreHealth(currentHealth);
        }

        // Guardar la vida actual en el SO
        if (playerDataSO != null)
        {
            playerDataSO.hp = currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        // Aquí puedes agregar lógica para manejar la muerte del jugador
        // Por ejemplo, deshabilitar el movimiento o enviar un mensaje de red
    }

 
   [PunRPC]
    public void SyncHealHealth(float newHealth)
    {
        currentHealth = newHealth;

        // Actualizar la barra de salud
        if (healthBar != null)
        {
            healthBar.RestoreHealth(currentHealth);
        }

        // Guardar la vida actual en el SO
        if (playerDataSO != null)
        {
            playerDataSO.hp = currentHealth;
        }
    }

    [PunRPC]
    public void SyncReduceHealth(float newHealth)
    {
        currentHealth = newHealth;

        // Actualizar la barra de salud
        if (healthBar != null)
        {
            healthBar.ReduceHealth(currentHealth);
        }

        // Guardar la vida actual en el SO
        if (playerDataSO != null)
        {
            playerDataSO.hp = currentHealth;
        }
    }

}
