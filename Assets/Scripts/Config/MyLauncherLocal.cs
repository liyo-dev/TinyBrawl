using UnityEngine;
using UnityEngine.Events;

public class MyLauncherLocal : MonoBehaviour
{
    public UnityEvent OnImpostorGameSelected;
    public UnityEvent OnBurguerGameSelected;
    void Start()
    {
        if (LocalOnlineOption.instance.SelectedMiniGame == Minigame.Impostor)
            OnImpostorGameSelected?.Invoke();
        else if (LocalOnlineOption.instance.SelectedMiniGame == Minigame.Burguer)
            OnBurguerGameSelected?.Invoke();
    }   
}
