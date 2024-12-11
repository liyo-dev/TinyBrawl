using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Service;

public class PlayerAttack : MonoBehaviourPunCallbacks
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Button specialAttackBtn;
    private Button attackBtnLeft;
    private Button attackBtnRight;

    [Header("Weapon Images")]
    public RawImage leftWeaponImage;
    public RawImage rightWeaponImage;

    [Header("Player Data")]
    private PlayerDataSO playerDataSO;

    [Header("Inventory Items")]
    public GameObject[] inventoryItems; // Lista de prefabs de ítems del inventario

    private void Awake()
    {
        var specialAttackBtnObj = GameObject.FindGameObjectWithTag("SpecialAttackBtn");
        if (specialAttackBtnObj != null)
        {
            specialAttackBtn = specialAttackBtnObj.GetComponent<Button>();
        }

        var attackBtnLeftObj = GameObject.FindGameObjectWithTag("AttackLeft");
        if (attackBtnLeftObj != null)
        {
            attackBtnLeft = attackBtnLeftObj.GetComponent<Button>();
        }

        var attackBtnRightObj = GameObject.FindGameObjectWithTag("AttackRight");
        if (attackBtnRightObj != null)
        {
            attackBtnRight = attackBtnRightObj.GetComponent<Button>();
        }
    }

    private void Start()
    {
        playerDataSO = ServiceLocator.GetService<PlayerDataService>().GetData();

        if (attackBtnLeft != null)
        {
            attackBtnLeft.onClick.AddListener(() => Attack("left"));
        }
        else
        {
            Debug.LogWarning("El botón de ataque izquierdo no se encontró en la escena.");
        }

        if (attackBtnRight != null)
        {
            attackBtnRight.onClick.AddListener(() => Attack("right"));
        }
        else
        {
            Debug.LogWarning("El botón de ataque derecho no se encontró en la escena.");
        }

        if (specialAttackBtn != null)
        {
            specialAttackBtn.onClick.AddListener(SpecialAttack);
        }
        else
        {
            Debug.LogWarning("El botón de ataque especial no se encontró en la escena.");
        }

        UpdateWeaponButtonLabels();
    }

    private void UpdateWeaponButtonLabels()
    {
        if (playerDataSO != null)
        {
            if (playerDataSO.leftHandItemId >= 0 && playerDataSO.leftHandItemId < inventoryItems.Length)
            {
                var leftWeaponPrefab = inventoryItems[playerDataSO.leftHandItemId];
                if (attackBtnLeft != null)
                {
                    var leftButtonLabel = attackBtnLeft.GetComponentInChildren<TMP_Text>();
                    if (leftButtonLabel != null)
                    {
                        leftButtonLabel.text = leftWeaponPrefab.name;
                    }
                }
            }

            if (playerDataSO.rightHandItemId >= 0 && playerDataSO.rightHandItemId < inventoryItems.Length)
            {
                var rightWeaponPrefab = inventoryItems[playerDataSO.rightHandItemId];
                if (attackBtnRight != null)
                {
                    var rightButtonLabel = attackBtnRight.GetComponentInChildren<TMP_Text>();
                    if (rightButtonLabel != null)
                    {
                        rightButtonLabel.text = rightWeaponPrefab.name;
                    }
                }
            }
        }
    }

    private void Attack(string hand)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            if (photonView.IsMine)
            {
                Debug.Log($"Ataque con la mano {hand} iniciado.");
                PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
            }
        }
    }

    private void SpecialAttack()
    {
        Debug.Log("Special Attack pressed");
    }
}
