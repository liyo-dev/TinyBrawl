using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class MartilloAction : MonoBehaviourPun
{
    public float speed = 10f; // Velocidad del martillo
    public float maxDistance = 20f; // Distancia máxima antes de regresar
    private Transform playerTransform;

    private Vector3 initialPosition;

    private void Start()
    {
        // Solo busca el jugador si este objeto pertenece al cliente local
        if (photonView.IsMine)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("No se encontró ningún jugador con el tag 'Player'.");
            }
        }
    }

    public void DoStart()
    {
        if (!photonView.IsMine) return; // Solo controla el movimiento el cliente propietario

        Debug.Log("Entro");

        initialPosition = transform.position;

        // Mover el martillo hacia adelante
        Vector3 targetPosition = initialPosition + transform.forward * maxDistance;

        // Animar el martillo hacia adelante y sincronizar el movimiento en la red
        transform.DOMove(targetPosition, maxDistance / speed)
            .SetEase(Ease.Linear)
            .OnComplete(() => photonView.RPC(nameof(ReturnToPlayer), RpcTarget.All)).Play(); // Llama al RPC para todos los clientes
    }

    [PunRPC]
    private void ReturnToPlayer()
    {
        if (playerTransform == null) return;

        // Regresar el martillo al jugador
        transform.DOMove(playerTransform.position, Vector3.Distance(transform.position, playerTransform.position) / speed)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.position = initialPosition;
            }).Play();
    }
}
