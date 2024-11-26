using Service;
using UnityEngine;

public class GameBurguerOptions : MonoBehaviour
{
    private GameBurguer _gameBurguer;
    private GameBurguerLocal _gameBurguerLocal;

    public void CheckPlayerTurn(int element)
    {
        if (ServiceLocator.GetService<LocalOnlineOption>().IsOnline())
        {
            if (_gameBurguer == null) _gameBurguer = FindObjectOfType<GameBurguer>();

            _gameBurguer.CheckPlayerTurn(element);
        }
        else
        {
            if (_gameBurguerLocal == null) _gameBurguerLocal = FindObjectOfType<GameBurguerLocal>();

            _gameBurguerLocal.CheckPlayerTurn(element);
        }

    }
}
