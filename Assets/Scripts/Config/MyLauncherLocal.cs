using Service;
using UnityEngine;
using UnityEngine.Events;

public class MyLauncherLocal : MonoBehaviour
{
    public UnityEvent OnImpostorGameSelected;
    public UnityEvent OnBurguerGameSelected;
    void Start()
    {
        if (ServiceLocator.GetService<LocalOnlineOption>().IsImpostor())
        {
            OnImpostorGameSelected?.Invoke();
        }
        else if (ServiceLocator.GetService<LocalOnlineOption>().IsBurguer())
        {
            OnBurguerGameSelected?.Invoke();
        }
    }   
}
