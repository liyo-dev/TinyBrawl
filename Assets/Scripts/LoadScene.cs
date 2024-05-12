using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    public void LoadNextScene()
    {
        // Si no se proporciona un nombre de escena siguiente, no se hace nada
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("No se proporcionó el nombre de la siguiente escena.");
            return;
        }

        // Carga la siguiente escena
        SceneManager.LoadScene(nextSceneName);
    }
}