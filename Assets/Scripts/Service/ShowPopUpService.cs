using UnityEngine;
using UnityEngine.Events;

public class ShowPopUpService : MonoBehaviour
{
    [SerializeField] private string titleText; // T�tulo del popup
    [SerializeField] private string subtitleText; // Subt�tulo del popup

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
