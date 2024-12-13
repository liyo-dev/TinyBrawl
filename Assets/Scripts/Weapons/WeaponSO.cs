using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Inventory/Weapon", order = 1)]
public class WeaponSO : ScriptableObject
{
    public float attackRange = 1.5f; // Rango del ataque
    public float attackDamage = 20; // Da�o que inflige el ataque
    public string weaponName; // Nombre del arma
    public string description; // Descripci�n del arma
    public GameObject effectPrefab; // Prefab del efecto del arma
    public float effectDuration; // Duraci�n del arma
    public float cooldown; // Tiempo de recarga del arma
    public WeaponType weaponType;
}