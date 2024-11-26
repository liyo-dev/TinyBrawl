using Service;
using UnityEngine;
using UnityEngine.Events;

public class GetGameOption : MonoBehaviour
{
    public UnityEvent OnLocalEvent;
    public UnityEvent OnOnlineEvent;

    public void Start()
    {
        if (ServiceLocator.GetService<LocalOnlineOption>().SelectedTypePlayer == TypePlayer.Local)
        {
            OnLocalEvent?.Invoke();
        }
        else
        {
            OnOnlineEvent?.Invoke();
        }
    }
}
