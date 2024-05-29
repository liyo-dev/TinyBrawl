using UnityEngine;
using DG.Tweening;
public class DOTMoveDown : MonoBehaviour
{
    public GameObject NextFrame;
    void Start()
    {
        transform.DOMoveY(-180f, 4).OnComplete(() =>
        {
            NextFrame.SetActive(true);
        }).SetRelative().Play();
    }
}
