using UnityEngine;
using DG.Tweening;

public class DOTRingUI : MonoBehaviour
{
    private float rotationDuration = 2f;
    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear) 
            .SetLoops(-1, LoopType.Incremental) 
            .Play(); 
    }
    
}
