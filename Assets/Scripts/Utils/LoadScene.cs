using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private SceneNames nextSceneName = SceneNames.None;

    public void LoadInstant()
    {
        if (nextSceneName == SceneNames.None)
        {
            Debug.LogWarning("No se proporcionó un nombre de escena válido.");
            return;
        }

        SceneManager.LoadScene(nextSceneName.ToString());
    }

    public void LoadAsync()
    {

    }
}
