using UnityEngine;

[CreateAssetMenu(fileName = "Characters", menuName = "Game/Characters", order = 1)]
public class CharactersSO : ScriptableObject
{
    public GameObject[] characters;
}
