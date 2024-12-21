using Photon.Pun;
using UnityEngine;

public class InstantiatePhotonGameObject : MonoBehaviour
{
    public GameObject prefab;

    void Start()
    {
        PhotonNetwork.Instantiate(prefab.name, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
