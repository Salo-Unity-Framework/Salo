using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// Base class for Bootstrap resource loaders. Note that base classes in BootstrapScene
    /// will add themselves to BootstrapRuntimeData even if the component is disabled.
    /// </summary>
    public abstract class BootstrapResourceLoaderBase : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            InfrastructureSOHolder.Instance.BootstrapRuntimeData.BootstrapResourceLoaders.Add(this);
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying)
            {
                InfrastructureSOHolder.Instance.BootstrapRuntimeData.BootstrapResourceLoaders.Remove(this);
            }
        }

        // Should be implemented as an async method
        public abstract UniTask Load();
    }
}
