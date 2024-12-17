using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    private PlayerHealthBar healthBar;

    private void Start()
    {
        healthBar = GetComponent<PlayerHealthBar>();
    }

    public void TakeDamage(float damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damage;

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            healthBar.ReduceHealth(currentHealth);

            Debug.Log($"Recibiste da�o: {damage}. Vida restante: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    [PunRPC]
    public void TakeDamageRPC(float damage)
    {
        TakeDamage(damage);
    }

    private void Die()
    {
        Debug.Log("Jugador ha muerto.");
        // Agregar l�gica de muerte, como deshabilitar controles, mostrar UI, etc.
    }
}