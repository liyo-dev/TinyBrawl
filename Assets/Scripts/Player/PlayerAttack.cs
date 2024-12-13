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

    private PlayerDataSO playerDataSO;

    private Weapon leftHandWeapon;
    private Weapon rightHandWeapon;

    private Collider[] meleeHitResults = new Collider[10];

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

        firePoint = transform.Find("FirePoint");

        // Recuperar las armas equipadas
        EquipWeapons();

        // Recuperar el nombre de las armas para los botones
        UpdateWeaponButtonLabels();

        if (attackBtnLeft != null)
        {
            attackBtnLeft.onClick.AddListener(() => UseWeapon(leftHandWeapon, attackBtnLeft));
        }

        if (attackBtnRight != null)
        {
            attackBtnRight.onClick.AddListener(() => UseWeapon(rightHandWeapon, attackBtnRight));
        }
    }

    private void EquipWeapons()
    {
        // Usar el InventorySO para obtener las armas equipadas
        if (playerDataSO.inventory == null)
        {
            Debug.LogWarning("El inventario del jugador no está asignado.");
            return;
        }

        var inventoryItems = playerDataSO.inventory.inventoryItems;

        GameObject leftHandItem = playerDataSO.leftHandItemId >= 0 && playerDataSO.leftHandItemId < inventoryItems.Count
            ? inventoryItems[playerDataSO.leftHandItemId]
            : null;

        GameObject rightHandItem = playerDataSO.rightHandItemId >= 0 && playerDataSO.rightHandItemId < inventoryItems.Count
            ? inventoryItems[playerDataSO.rightHandItemId]
            : null;

        if (leftHandItem != null)
        {
            leftHandWeapon = leftHandItem.GetComponent<Weapon>();
        }

        if (rightHandItem != null)
        {
            rightHandWeapon = rightHandItem.GetComponent<Weapon>();
        }
    }

    private void UpdateWeaponButtonLabels()
    {
        if (playerDataSO == null || playerDataSO.inventory == null) return;

        var inventoryItems = playerDataSO.inventory.inventoryItems;

        if (playerDataSO.leftHandItemId >= 0 && playerDataSO.leftHandItemId < inventoryItems.Count)
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

        if (playerDataSO.rightHandItemId >= 0 && playerDataSO.rightHandItemId < inventoryItems.Count)
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

    private void UseWeapon(Weapon weapon, Button attackButton)
    {
        if (weapon != null && photonView.IsMine)
        {
            switch (weapon.weaponData.weaponType)
            {
                case WeaponType.Shoot:
                    if (photonView.IsMine)
                    {
                        var meleeEffect = PhotonNetwork.Instantiate(weapon.weaponData.effectPrefab.name, firePoint.position, firePoint.rotation);

                        WeaponVisuals(weapon);

                        PerformMeleeAttack(weapon);

                        StartCoroutine(DestroyEffectAfterDelay(meleeEffect, weapon.weaponData.effectDuration));
                    }
                    break;

                case WeaponType.Ranged:
                    if (photonView.IsMine)
                    {
                        var meleeEffect = PhotonNetwork.Instantiate(weapon.weaponData.effectPrefab.name, transform.position, firePoint.rotation);

                        WeaponVisuals(weapon);

                        StartCoroutine(DestroyEffectAfterDelay(meleeEffect, weapon.weaponData.effectDuration));
                    }
                    break;

                case WeaponType.Defensive:
                    if (photonView.IsMine)
                    {
                        var defensiveEffect = PhotonNetwork.Instantiate(weapon.weaponData.effectPrefab.name, transform.position, transform.rotation);
                        
                        StartCoroutine(DestroyEffectAfterDelay(defensiveEffect, weapon.weaponData.effectDuration));
                        
                        photonView.RPC(nameof(SyncDefensiveEffect), RpcTarget.AllBuffered, defensiveEffect.GetComponent<PhotonView>().ViewID);
                    }
                    break;



                default:
                    Debug.LogWarning("Tipo de arma no manejado.");
                    break;
            }

            // Cooldown
            StartCoroutine(Cooldown(attackButton, weapon.weaponData.cooldown));
        }
    }

    private void WeaponVisuals(Weapon weapon)
    {
        string actionComponentName = weapon.weaponData.name + "Action";

        var actionComponent = weapon.gameObject.GetComponent(actionComponentName);

        if (actionComponent != null)
        {
            var method = actionComponent.GetType().GetMethod("DoStart");
            if (method != null)
            {
                method.Invoke(actionComponent, null);
            }
        }
    }

    private void PerformMeleeAttack(Weapon weapon)
    {
        float attackRange = weapon.weaponData.attackRange;

        // Detectar colisiones dentro del rango
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRange, meleeHitResults);

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = meleeHitResults[i];

            if (hit.CompareTag("Player") && hit.gameObject != gameObject) // Asegurarse de no golpear al jugador mismo
            {
                PlayerHealth targetHealth = hit.GetComponent<PlayerHealth>();

                if (targetHealth != null)
                {
                    Debug.Log($"Golpeando a: {hit.gameObject.name} con {weapon.weaponData.attackDamage} de daño.");
                    targetHealth.TakeDamage(weapon.weaponData.attackDamage);
                }
            }
        }
    }

    private System.Collections.IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null && effect.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("Destruyendo efecto después de la duración");
            PhotonNetwork.Destroy(effect);
        }
    }


    private System.Collections.IEnumerator Cooldown(Button button, float cooldownTime)
    {
        button.interactable = false;
        var fillImage = button.GetComponent<Image>();
        if (fillImage != null)
        {
            fillImage.fillAmount = 0;
        }

        float elapsedTime = 0f;

        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            if (fillImage != null)
            {
                fillImage.fillAmount = elapsedTime / cooldownTime;
            }
            yield return null;
        }

        button.interactable = true;
        if (fillImage != null)
        {
            fillImage.fillAmount = 1;
        }
    }

    [PunRPC]
    private void SyncDefensiveEffect(int effectViewID)
    {
        var defensiveEffect = PhotonView.Find(effectViewID)?.gameObject;

        if (defensiveEffect != null)
        {
            // Ajustar el efecto como hijo del jugador correspondiente
            defensiveEffect.transform.SetParent(transform, true);
            defensiveEffect.transform.localPosition = Vector3.zero; // Centrar en el jugador
            defensiveEffect.transform.localRotation = Quaternion.identity; // Asegurar rotación predeterminada
            defensiveEffect.transform.localScale = Vector3.one; // Resetear la escala
        }
        else
        {
            Debug.LogWarning("No se encontró el PhotonView para el efecto defensivo.");
        }
    }

}
 