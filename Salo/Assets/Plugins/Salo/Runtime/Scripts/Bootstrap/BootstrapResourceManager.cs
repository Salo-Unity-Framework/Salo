using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This bootstrapped system loads and unloads bootstrapped resources. The load method is run by
    /// AppBootstrap during the bootstrap process. Loaders in BootstrapSCene will register
    /// themselves during BootstrapScene load to be used by this class.
    public class BootstrapResourceManager : StaticInstanceOf<BootstrapResourceManager>
    {
        public async UniTask Load()
        {
            var loaders = InfrastructureSOHolder.Instance.BootstrapRuntimeData.BootstrapResourceLoaders;
            int actualLoaderCount = 0;

            for (int i = 0; i < loaders.Count; i++)
            {
                if (null == loaders[i]) continue; // Avoid invalid tasks

                // Wait for each loader to complete before moving to the next one.
                // This allows loaders to batch loading across multiple frames.
                await loaders[i].Load();
                actualLoaderCount++;
            }

            Debug.Log($"Bootstrap resource loading complete. Processed {actualLoaderCount}/{loaders.Count} loaders. Removing loader components...");

            // Remove loader components to save resources
            for (int i = loaders.Count - 1; i >= 0; i--)
            {
                Destroy(loaders[i]); // Destroy the component, not the game object
            }
        }
    }
}
