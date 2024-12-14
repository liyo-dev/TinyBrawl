using UnityEngine;

public class CharacterDataService : MonoBehaviour
{
    public static CharacterDataService instance;

    [SerializeField] private CharactersSO characterDataSO;

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

    public CharactersSO GetData()
    {
        return characterDataSO;
    }
}
