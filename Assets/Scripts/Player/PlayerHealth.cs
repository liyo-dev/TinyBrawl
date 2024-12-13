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
