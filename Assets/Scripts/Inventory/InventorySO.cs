using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Player Inventory", order = 1)]
public class InventorySO : ScriptableObject
{
    public List<GameObject> inventoryItems = new List<GameObject>(); // Lista de objetos del inventario
}
