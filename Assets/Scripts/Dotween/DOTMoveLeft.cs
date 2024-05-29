using DG.Tweening;
using UnityEngine;

public class DOTMoveLeft : MonoBehaviour
{
    public GameObject NextFrame;
    void Start()
    {
        transform.DOMoveX(-180f, 4).OnComplete(() =>
        {
            NextFrame.SetActive(true);
        }).SetRelative().Play();
    }
}
