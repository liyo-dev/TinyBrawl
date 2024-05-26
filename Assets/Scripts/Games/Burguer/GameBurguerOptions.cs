using UnityEngine;

public class GameBurguerOptions : MonoBehaviour
{
    private GameBurguer _gameBurguer;

    public void CheckPlayerTurn(int element)
    {
        if (_gameBurguer == null) _gameBurguer = FindObjectOfType<GameBurguer>();
        _gameBurguer.CheckPlayerTurn(element);
    }
}
