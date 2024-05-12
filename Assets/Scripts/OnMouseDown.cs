using UnityEngine;
using UnityEngine.Events;

public class ClickHandler : MonoBehaviour
{
    public UnityEvent OnMouseDownEvent;
    private void OnMouseDown()
    {
        OnMouseDownEvent?.Invoke();
    }
}