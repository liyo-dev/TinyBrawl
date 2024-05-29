using DG.Tweening;
using UnityEngine;

public class DOTControlsJump : MonoBehaviour
{
    public Transform target;
    public float jumpHeight = 13f;
    public float jumpDuration = 0.5f;

    public void JumpSequence()
    {
        target.DOMoveY(jumpHeight, jumpDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            target.DOMoveY(0f, 0.2f).SetEase(Ease.InQuad);
        });
    }
}
