using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This exists on the AppBootstrap prefab. This script takes over the app flow during
    /// bootstrapping. It starts from its Start method and ends when it calls
    /// SceneLoadEvents.FirstSceneLoadRequested. Frameowrk users may subclass
    /// this to override bootstrap() to implement their own bootstrap logic.
    /// </summary>
    public class AppBootstrap : MonoBehaviour
    {
        // This is the entry point of the bootstrap flow
        private void Start()
        {
            bootstrap().Forget();
        }

        // Override this to add your own bootstrap logic. DOn't forget to call base.bootstrap();
        protected virtual async UniTaskVoid bootstrap()
        {
            // Check entitlement etc

            // Play splash media

            // Load bootstrapped resources
            await BootstrapResourceManager.Instance.Load();

            SceneLoadEvents.FirstSceneLoadRequested();
        }
    }
}
