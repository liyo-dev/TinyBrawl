using Service;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOptionsServiceLocal : MonoBehaviour
{
    private LocalOnlineOption localOnlineOption;
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
        SceneManager.LoadScene(SceneNames.Game.ToString());
    }
}
