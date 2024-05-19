using UnityEngine;
using DG.Tweening;


public class DOTWings : MonoBehaviour
{
    float rotationDuration = 1f; 
    void Start()
    {
        transform.DORotate(new Vector3(0f, 0f, -360f), rotationDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
}
