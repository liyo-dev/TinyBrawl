using UnityEngine;

public class LocalOnlineOption : MonoBehaviour
{
    public static TypePlayer SelectedTypePlayer = TypePlayer.Local;
    public Minigame SelectedMiniGame;

    public static LocalOnlineOption instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        instance = this;
        
        DontDestroyOnLoad(gameObject); 
    }

    public void SetOnlineGame()
    {
        SelectedTypePlayer = TypePlayer.Online;
    }
    
    public void SetLocalGame()
    {
        SelectedTypePlayer = TypePlayer.Local;
    }
    
    public void SetImpostorGame()
    {
        SelectedMiniGame = Minigame.Impostor;
    }
    
    public void SetBurguerGame()
    {
        SelectedMiniGame = Minigame.Burguer;
    }
    
    public bool IsOnline() => SelectedTypePlayer == TypePlayer.Online;
}