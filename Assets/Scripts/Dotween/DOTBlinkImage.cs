using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DOTBlinkImage : MonoBehaviour
{
    public Image imageToBlink;
    public float blinkDuration = 0.5f;

    void Start()
    {
        BlinkImage();
    }

    void BlinkImage()
    {
        imageToBlink.DOFade(0f, blinkDuration).SetLoops(-1, LoopType.Yoyo).Play();
    }
}
