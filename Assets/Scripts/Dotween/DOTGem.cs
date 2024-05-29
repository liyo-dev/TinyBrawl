using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DOTGem : MonoBehaviour
{
    private float rotationDuration = 1f; // Duración de la rotación
    private float jumpHeight = 0.5f; // Altura del salto
    private float jumpDuration = 0.5f; // Duración del salto

    private void Start()
    {
        AnimateGem();
    }

    private void AnimateGem()
    {
        transform.DORotate(new Vector3(0f, 360f, 0f), rotationDuration, RotateMode.FastBeyond360).SetLoops(-1);
        transform.DOLocalJump(transform.localPosition, jumpHeight, 1, jumpDuration).SetLoops(-1);
    }
}
