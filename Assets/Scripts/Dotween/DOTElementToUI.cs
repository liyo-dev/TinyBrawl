using UnityEngine;
using DG.Tweening;

public class DOTElementToUI : MonoBehaviour
{
    public float shrinkSpeed = 1f;
    public Vector2 targetPosition = new Vector2(1, 1);

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void StartShrinkAndMove()
    {
        // Obtener el tamaño original del objeto
        Vector2 originalSize = rectTransform.sizeDelta;

        // Calcular el tamaño final como un pequeño porcentaje del tamaño original
        Vector2 targetSize = originalSize * 0.1f;

        // Realizar la animación de encojimiento y movimiento utilizando DOTween
        rectTransform.DOSizeDelta(targetSize, shrinkSpeed)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Asegurarse de que el objeto tenga el tamaño y la posición final
                rectTransform.sizeDelta = targetSize;
                rectTransform.anchoredPosition = targetPosition;
            });
        rectTransform.DOAnchorPos(targetPosition, shrinkSpeed)
            .SetEase(Ease.OutQuad);
    }
}