using DG.Tweening;
using UnityEngine;

public class DOTRectTransformScale: MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector3 escalaFinal = new Vector3(2f, 2f, 2f);
    [SerializeField] private float duracion = 1f;
    public bool Automatic = true;

    private void Start()
    {
        if (rectTransform == null)
        {
            Debug.LogError("El RectTransform no está asignado en el Inspector.");
            return;
        }
        
        if (Automatic) 
            EscalarConTween();
    }

    public void EscalarConTween()
    {
        Vector3 escalaInicial = rectTransform.localScale;
        
        rectTransform.DOScale(escalaFinal, duracion)
            .SetEase(Ease.OutElastic) 
            .OnComplete(() => Debug.Log("Escalado completado")).Play();
    }   
}
