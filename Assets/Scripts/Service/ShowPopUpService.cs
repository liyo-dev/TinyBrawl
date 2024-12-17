using UnityEngine;
using UnityEngine.Events;

public class ShowPopUpService : MonoBehaviour
{
    [SerializeField] private string titleText; // Título del popup
    [SerializeField] private string subtitleText; // Subtítulo del popup

    public UnityEvent OnyesAnswered;
    public UnityEvent OnNoAnswered;
    public void Show()
    {
        PopUp.Instance.Show(titleText, subtitleText, OnyesResponse, OnNoResponse);
    }

    void OnyesResponse()
    {
        OnyesAnswered?.Invoke();

    }

    void OnNoResponse()
    {
        OnNoAnswered?.Invoke();
    }
}
