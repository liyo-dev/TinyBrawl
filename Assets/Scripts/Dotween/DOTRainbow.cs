using UnityEngine;
using DG.Tweening;

public class DOTRainbow : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    
    public float height;
    public float duration;

    private void Start()
    {
        Vector3[] path = new Vector3[3];
        path[0] = startPoint.position;
        path[1] = new Vector3((startPoint.position.x + endPoint.position.x) / 2f, height, (startPoint.position.z + endPoint.position.z) / 2f);
        path[2] = endPoint.position;

        transform.DOPath(path, duration, PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.OutQuad);
    }
}
