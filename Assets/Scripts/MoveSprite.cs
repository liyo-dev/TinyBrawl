using System.Collections;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class MoveSprite : MonoBehaviourPunCallbacks
{
    public GameObject WaitForOthers;
    public float moveDuration = 5f;
    public bool canMove;
    public float minX = -5f; // Límites mínimos en X
    public float maxX = 5f; // Límites máximos en X
    public float minY = -5f; // Límites mínimos en Y
    public float maxY = 5f; // Límites máximos en Y
    
    public void DoStart()
    {
        if (photonView.IsMine)
        {
            canMove = true;
            WaitForOthers.SetActive(false);
            StartCoroutine(MoveRoutine());
        }
    }

    private IEnumerator MoveRoutine()
    {
        while (canMove)
        {
            // Generar una posición aleatoria dentro de los límites especificados
            Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);

            // Mover hacia la posición aleatoria
            photonView.RPC("MoveToPosition", RpcTarget.All, randomPosition, moveDuration);
            yield return new WaitForSeconds(moveDuration);
        }
    }

    [PunRPC]
    private void MoveToPosition(Vector3 position, float duration)
    {
        transform.DOMove(position, duration).SetEase(Ease.Linear);
    }
}