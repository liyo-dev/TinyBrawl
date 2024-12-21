using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Status")]
    public bool isStartRoom;
    public bool isLootRoom;
    public bool isEnemyRoom;

    [Header("Doors")]
    public GameObject DoorD;
    public GameObject DoorU;
    public GameObject DoorL;
    public GameObject DoorR;

    [Header("Insides")]
    public Material[] Materials;
    public GameObject[] Enemies;
    public GameObject[] Loot;

    [Header("Light")]
    public GameObject Lights;
    public bool isRandomized;

    private void Start()
    {
        SetMaterials();
        SetLoot();
        SetEnemies();

        if (Lights != null && isRandomized)
        {
            int countToLeave = Random.Range(0, Lights.transform.childCount + 1);

            while (Lights.transform.childCount > countToLeave)
            {
                Transform childToDestroy = Lights.transform.GetChild(Random.Range(0, Lights.transform.childCount));
                DestroyImmediate(childToDestroy.gameObject);
            }
        }
    }

    private void OnValidate()
    {
        SetMaterials();
    }

    public void SetMaterials()
    {
        Material[] _materials = Materials;

        if (Application.isPlaying && RoomsPlacer.Instance != null && RoomsPlacer.Instance.overrideMaterials)
        {
            _materials = RoomsPlacer.Instance.Materials;
        }

        if (_materials != null && _materials.Length > 0)
        {
            GameObject[] blocks = FindObjectsWithTag(transform, "Dungeon Cube");

            foreach (var block in blocks)
            {
                MeshRenderer renderer = block.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = _materials[Random.Range(0, _materials.Length)];
                }
            }
        }
        else
        {
            Debug.LogWarning("No materials found to set.");
        }
    }

    public void SetLoot()
    {
        if (isLootRoom)
        {
            GameObject[] _loot;

            if (Application.isPlaying && RoomsPlacer.Instance != null && RoomsPlacer.Instance.overrideLoot)
                _loot = RoomsPlacer.Instance.LootPrefabs;
            else
                _loot = Loot;

            if (_loot != null && _loot.Length > 0)
            {
                PhotonNetwork.Instantiate(_loot[Random.Range(0, _loot.Length)].name, transform.position, Quaternion.Euler(0, 180, 0));
            }
            else
            {
                Debug.LogWarning("No loot found to set.");
            }
        }
    }

    public void SetEnemies()
    {
        if (isEnemyRoom)
        {
            GameObject[] _enemies;

            if (Application.isPlaying && RoomsPlacer.Instance != null && RoomsPlacer.Instance.overrideEnemies)
                _enemies = RoomsPlacer.Instance.EnemiesPrefabs;
            else
                _enemies = Enemies;

            if (_enemies != null && _enemies.Length > 0)
            {
                PhotonNetwork.Instantiate(_enemies[Random.Range(0, _enemies.Length)].name, transform.position, Quaternion.Euler(0, 180, 0));
            }
            else
            {
                Debug.LogWarning("No enemies found to set.");
            }
        }
    }

    public GameObject[] FindObjectsWithTag(Transform parent, string tag)
    {
        List<GameObject> taggedGameObjects = new List<GameObject>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.CompareTag(tag))
                taggedGameObjects.Add(child.gameObject);
            if (child.childCount > 0)
                taggedGameObjects.AddRange(FindObjectsWithTag(child, tag));
        }

        return taggedGameObjects.ToArray();
    }
}
