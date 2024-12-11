using EasyTransition;
using Service;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOptionsServiceLocal : MonoBehaviour
{
    private LocalOnlineOption localOnlineOption;
    public TransitionSettings transition;
    private void Start()
    {
        localOnlineOption = ServiceLocator.GetService<LocalOnlineOption>();
    }

    public void Impostor()
    {
        localOnlineOption.SetImpostorGame();

    }

    public void Burguer()
    {
        localOnlineOption.SetBurguerGame();
    }

    public void LoadGame()
    {
        ServiceLocator.GetService<TransitionManager>().Transition(SceneNames.Game.ToString(), transition, 0);
    }
}
