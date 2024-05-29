using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DOTTitleUp : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public float fadeInDuration = 3f;
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
        
        transform.DOMoveY(2f, 3).Play();
        _spriteRenderer.DOFade(1f, fadeInDuration).Play();
    }
}
