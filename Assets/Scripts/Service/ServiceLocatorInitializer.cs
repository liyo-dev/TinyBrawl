
using EasyTransition;
using UnityEngine;

namespace Service
{
    [DefaultExecutionOrder(-1000)]
    public class ServiceLocatorInitializer : MonoBehaviour
    {
        private void Awake()
        {
            if (!ServiceLocator.IsInitialized)
            {
                AddServices();
                ServiceLocator.IsInitialized = true;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void AddServices()
        {
            var localOnlineOption = FindFirstObjectByType<LocalOnlineOption>();
            ServiceLocator.AddService(localOnlineOption);

            var transitionManager = FindFirstObjectByType<TransitionManager>();
            ServiceLocator.AddService(transitionManager);

            var playerDataSO = FindFirstObjectByType<PlayerDataService>();
            ServiceLocator.AddService(playerDataSO);

            var characterDataSO = FindFirstObjectByType<CharacterDataService>();
            ServiceLocator.AddService(characterDataSO);
        }
    }
}