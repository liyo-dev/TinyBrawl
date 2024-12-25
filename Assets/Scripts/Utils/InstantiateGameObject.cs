using UnityEngine;

public class InstantiateGameObject : MonoBehaviour
{
    public GameObject prefab;
    void Start()
    {
        Instantiate(prefab, Vector3.zero, Quaternion.identity);   
    }
}
