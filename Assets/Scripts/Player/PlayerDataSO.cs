using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerDataSO : ScriptableObject
{
    public string username;    // Username del jugador
    public int level;          // Nivel del jugador
    public int points;         // Puntos del jugador
    public int selectedCharacterId; // Id del personaje seleccionado
    public float hp;
}
