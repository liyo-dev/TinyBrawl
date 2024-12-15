using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviourPunCallbacks
{
    [Header("Portal Settings")]
    [SerializeField] private string waitingRoomSceneName = "WaitingRoom"; // Nombre de la sala de espera

    private GameObject playerGameObject;

    private bool isPhotonViewMine = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra al portal es un jugador
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log($"Jugador {other.name} ha entrado al portal.");

            isPhotonViewMine = true;

            playerGameObject = other.gameObject;

            PopUp.Instance.Show("Entrar al laberinto", "¿Estás seguro de continuar?", TransferToWaitingRoom, OnNoAction);
        }
    }

    public void TransferToWaitingRoom()
    {
        if (isPhotonViewMine)
        {
            //GameObject leftHandObject = GameObject.FindWithTag("EquipLeft");
            //GameObject rightHandObject = GameObject.FindWithTag("EquipRight");

            PhotonNetwork.Destroy(playerGameObject);
            //PhotonNetwork.Destroy(leftHandObject);
            //PhotonNetwork.Destroy(rightHandObject);

            SceneManager.LoadScene(SceneNames.WaitingRoom.ToString());
        }
    }


    private void OnNoAction()
    {
        Debug.Log("El jugador seleccionó 'No'.");
    }
}
