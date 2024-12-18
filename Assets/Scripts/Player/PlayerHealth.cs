using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public GameObject deadFeedback;

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
        GetComponent<PlayerMovement>().enabled = false;
        transform.DOLocalMoveX(transform.localPosition.x + 1, .2f)
            .SetLoops(3, LoopType.Yoyo) // Repite en modo Yoyo para moverse de ida y vuelta
            .SetEase(Ease.InOutSine).Play().OnComplete(() =>
            {
                GameObject explosion = PhotonNetwork.Instantiate(deadFeedback.name, transform.position, Quaternion.identity);
                explosion.transform.localScale = new Vector3(5, 5, 5);
                Invoke(nameof(DestroyObject), .2f);
            }); ;
    }

    private void DestroyObject()
    {
        PhotonNetwork.Destroy(gameObject);
        PopUp.Instance.Show("Has perdido", "¿Salir al Menú?", OnYes, OnNo);
    }

    void OnYes()
    {
        SceneManager.LoadScene(SceneNames.Menu.ToString());
    }

    void OnNo()
    {
        SceneManager.LoadScene(SceneNames.World.ToString());
    }
}
