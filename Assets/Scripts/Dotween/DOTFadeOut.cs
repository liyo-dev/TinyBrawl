using System.Collections;
using DG.Tweening;
using UnityEngine;

public class DOTFadeOut : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public float fadeOutDuration = 1f;
    public float delay = 0f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(nameof(Delay));
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        
        Color spriteColor = _spriteRenderer.color;
        spriteColor.a = 1f;
        _spriteRenderer.color = spriteColor;
        
        _spriteRenderer.DOFade(0f, fadeOutDuration).Play();
    }
}