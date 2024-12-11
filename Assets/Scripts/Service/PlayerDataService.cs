using UnityEngine;

public class PlayerDataService : MonoBehaviour
{
    public static PlayerDataService instance;

    [SerializeField] private PlayerDataSO playerDataSO;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public PlayerDataSO GetData()
    {
        return playerDataSO;
    }
}
