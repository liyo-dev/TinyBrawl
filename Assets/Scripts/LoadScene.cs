using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    public void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("No se proporcionó el nombre de la siguiente escena.");
            return;
        }
        
        SceneManager.LoadScene(nextSceneName);
    }
}