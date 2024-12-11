using EasyTransition;
using Service;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private SceneNames nextSceneName = SceneNames.None;
    public TransitionSettings transition;
    public float startDelay;

    public void LoadInstant()
    {
        if (nextSceneName == SceneNames.None)
        {
            Debug.LogWarning("No se proporcionó un nombre de escena válido.");
            return;
        }

        if (transition == null)
        {
            Debug.LogError("Hay que asignar una transición");
            return;
        }

        ServiceLocator.GetService<TransitionManager>().Transition(nextSceneName.ToString(), transition, startDelay);
    }
}
