using UnityEngine;
using DG.Tweening;
using TMPro;

public class DOTTextFadeOut : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float fadeOutDuration = 1.0f;
    public float delay = 0.0f;

    void Start()
    {
        if (textMeshPro == null)
        {
            Debug.LogError("El componente TextMeshProUGUI no está asignado en el Inspector.");
            return;
        }
        
        FadeOut();
    }

    void FadeOut()
    {
        Color textColor = textMeshPro.color;
        textColor.a = 1f;
        textMeshPro.color = textColor;

        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.AppendInterval(delay); 
        fadeOutSequence.Append(textMeshPro.DOFade(0f, fadeOutDuration)); 

        fadeOutSequence.Play();
    }
}
