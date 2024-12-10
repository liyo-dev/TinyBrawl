using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviourPunCallbacks
{
    public GameObject bulletPrefab;
    public Transform firePoint; 
    private Button specialAttackBtn;
    private Button attackBtn;

    private void Awake()
    {
        var specialAttackBtnObj = GameObject.FindGameObjectWithTag("SpecialAttackBtn");
        if (specialAttackBtnObj != null)
        {
            specialAttackBtn = specialAttackBtnObj.GetComponent<Button>();
        }

        var attackBtnObj = GameObject.FindGameObjectWithTag("AttackBtn");
        if (attackBtnObj != null)
        {
            attackBtn = attackBtnObj.GetComponent<Button>();
        }
    }

    private void Start()
    {
        if (attackBtn != null)
        {
            attackBtn.onClick.AddListener(Attack);
        }
        else
        {
            Debug.LogWarning("El botón de ataque no se encontró en la escena.");
        }

        if (specialAttackBtn != null)
        {
            specialAttackBtn.onClick.AddListener(SpecialAttack);
        }
        else
        {
            Debug.LogWarning("El botón de ataque especial no se encontró en la escena.");
        }
    }


    private void Attack()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
            }
        }
    }

    private void SpecialAttack()
    {
        // Placeholder para el ataque especial
        Debug.Log("Special Attack pressed");
    }
}
