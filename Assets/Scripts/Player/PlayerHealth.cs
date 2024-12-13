using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviourPun
{
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    public void TakeDamage(float damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            Debug.Log($"Recibiste daño: {damage}. Vida restante: {currentHealth}");

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
        // Agregar lógica de muerte, como deshabilitar controles, mostrar UI, etc.
    }
}
