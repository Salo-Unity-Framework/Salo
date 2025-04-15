using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This exists on the Infrastructure prefab. This script takes over the
    /// app flow during bootstrapping. It starts from its Start method and
    /// ends when it calls SceneLoadEvents.FirstSceneLoadRequested.
    /// </summary>
    public class AppBootstrap : MonoBehaviour
    {
        // This is the entry point of the bootstrap flow
        private async void Start()
        {
            // Check entitlement etc

            // Play splash media

            // Load bootstrapped resources
            await BootstrapResourceManager.Instance.Load();

            SceneLoadEvents.FirstSceneLoadRequested();
        }
    }
}
