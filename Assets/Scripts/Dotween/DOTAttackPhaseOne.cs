using UnityEngine;
using DG.Tweening;

public class DOTAttackPhaseOne : MonoBehaviour
{
    public Transform _target;
    
    void Start()
    {
        transform.DOMove(_target.position, 1)
            .SetEase(Ease.Flash)
            .OnComplete(() => Destroy(gameObject));
    }
}
