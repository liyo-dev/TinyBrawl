using Service;
using UnityEngine;
using UnityEngine.Events;

public class ChooseGameLocalOnline : MonoBehaviour
{
    public UnityEvent OnLocalGame; 
    public UnityEvent OnOnlineGame; 
    void Start()
    {
        if (ServiceLocator.GetService<LocalOnlineOption>().IsOnline())
        {
            OnOnlineGame?.Invoke();
        } else
        {
            OnLocalGame?.Invoke();
        }
    }
}
