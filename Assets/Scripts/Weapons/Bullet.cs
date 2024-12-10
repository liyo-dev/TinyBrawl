using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    public float speed = 10f; // Velocidad de la bala
    public float lifetime = 3f; // Tiempo de vida de la bala antes de destruirse

    private void Start()
    {
        // Destruye la bala después de un tiempo para evitar que quede flotando en la escena
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Mueve la bala hacia adelante en función de su velocidad
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine) return;

        Debug.Log("Bullet hit: " + collision.gameObject.name);
        //Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine) return;
    }
}
