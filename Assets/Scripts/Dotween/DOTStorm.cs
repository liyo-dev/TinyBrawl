using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DOTStorm : MonoBehaviour
{
    public UnityEvent OnFlashCompleted;
    public Image uiImage;
    public float minFlashDuration = 0.3f;
    public float maxFlashDuration = 0.8f;
    public float minTimeBetweenFlashes = 1f;
    public float maxTimeBetweenFlashes = 5f;

    void Start()
    {
        StartCoroutine(StormRoutine());
    }

    IEnumerator StormRoutine()
    {
        while (true)
        {
            Flash();
            
            float waitTime = Random.Range(minTimeBetweenFlashes, maxTimeBetweenFlashes);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void Flash()
    {
        float randomDuration = Random.Range(minFlashDuration, maxFlashDuration);

        uiImage.DOFade(1, randomDuration).SetLoops(2, LoopType.Yoyo).OnComplete(() => OnFlashCompleted.Invoke()).Play();
    }
}