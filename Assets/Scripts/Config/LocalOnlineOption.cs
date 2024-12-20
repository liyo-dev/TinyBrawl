using UnityEngine;

public class LocalOnlineOption : MonoBehaviour
{
    public TypePlayer SelectedTypePlayer = TypePlayer.Local;

    private Minigame SelectedMiniGame;

    public bool NoLogin = false;

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
    public void SetFishingGame()
    {
        SelectedMiniGame = Minigame.Fishing;
    }

    public void SetWorldGame()
    {
        SelectedMiniGame = Minigame.World;
    }

    public void SetMazeGame()
    {
        SelectedMiniGame = Minigame.Maze;
    }

    public bool IsOnline() => SelectedTypePlayer == TypePlayer.Online;
    public bool IsBurguer() => SelectedMiniGame == Minigame.Burguer;
    public bool IsImpostor() => SelectedMiniGame == Minigame.Impostor;
    public bool IsFishing() => SelectedMiniGame == Minigame.Fishing;
    public bool IsWorld() => SelectedMiniGame == Minigame.World;
    public bool IsMaze() => SelectedMiniGame == Minigame.Maze;
}