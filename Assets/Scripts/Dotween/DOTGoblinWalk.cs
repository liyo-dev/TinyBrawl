using UnityEngine;
using DG.Tweening;

public class DOTGoblinWalk : MonoBehaviour
{
    private Tween currentTween;
    private float initialY;

    private void Start()
    {
        initialY = transform.position.y;
    }

    public void DoWalk()
    {
        currentTween = transform.DOMoveY(transform.position.y + 0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void DoStop()
    {
        if (currentTween != null && currentTween.IsPlaying())
        {
            currentTween.Kill();
            currentTween = null;
            transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
        }
    }

    public void DoRotate()
    {
        transform.DORotate(new Vector3(0f, 180f, 0f), 0.5f).SetEase(Ease.Linear).Play();
    }
}
