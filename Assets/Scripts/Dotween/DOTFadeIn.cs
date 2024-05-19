using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DOTFadeIn : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public float fadeInDuration = 1f;
    public float delay = 0f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Color spriteColor = _spriteRenderer.color;
        spriteColor.a = 0f;
        StartCoroutine(nameof(Delay));
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        
        Color spriteColor = _spriteRenderer.color;
        spriteColor.a = 0f;
        _spriteRenderer.color = spriteColor;
        
        _spriteRenderer.DOFade(1f, fadeInDuration).Play();
    }
}
