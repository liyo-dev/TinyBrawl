using UnityEngine;
using Photon.Pun;
using EasyTransition;
using UnityEngine.SceneManagement;
using Service;

public class LoadScenePun : MonoBehaviourPunCallbacks
{
    [SerializeField] private SceneNames nextSceneName = SceneNames.None;
    public TransitionSettings transition;
    public float startDelay;

    public void LoadInstant()
    {
        if (nextSceneName == SceneNames.None)
        {
            Debug.LogWarning("No se proporcion� un nombre de escena v�lido.");
            return;
        }

        if (transition == null)
        {
            Debug.LogError("Hay que asignar una transici�n");
            return;
        }

        photonView.RPC(nameof(LoadInstantRPC), RpcTarget.All);
    }


    public void LoadNoTransition()
    {
        if (nextSceneName == SceneNames.None)
        {
            Debug.LogWarning("No se proporcion� un nombre de escena v�lido.");
            return;
        }

        photonView.RPC(nameof(LoadNoTransitionRPC), RpcTarget.All);
    }

    [PunRPC]
    private void LoadInstantRPC()
    {
        ServiceLocator.GetService<TransitionManager>().Transition(nextSceneName.ToString(), transition, startDelay);
    }

    [PunRPC]
    private void LoadNoTransitionRPC()
    {
        Debug.Log("Entro");
        SceneManager.LoadScene(nextSceneName.ToString());
    }
}
