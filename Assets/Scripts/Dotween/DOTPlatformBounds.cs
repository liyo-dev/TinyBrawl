using System;
using DG.Tweening;
using UnityEngine;

public class DOTPlatformBounds : MonoBehaviour
{
    private Renderer boundsRenderer; // Referencia al Renderer de los límites
    public Color startColor; // Color inicial de los límites
    public Color endColor; // Color final (transparente) de los límites
    public float animationDuration = 2f; // Duración de la animación

    private void Awake()
    {
        boundsRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        boundsRenderer.material.color = startColor;

        boundsRenderer.material.DOColor(endColor, animationDuration)
            .SetEase(Ease.OutExpo)
            .OnComplete(() => boundsRenderer.material.color = endColor);
    }
}
