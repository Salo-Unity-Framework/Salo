using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// Base class for Scene resource loaders. Note that base classes in a loaded scene
    /// will add themselves to SceneLoadRuntimeData even if the component is disabled.
    /// </summary>
    public abstract class SceneResourceLoaderBase : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            InfrastructureSOHolder.Instance.SceneLoadRuntimeData.SceneResourceLoaders.Add(this);
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying)
            {
                InfrastructureSOHolder.Instance.SceneLoadRuntimeData.SceneResourceLoaders.Remove(this);
            }
        }

        // Should be implemented as async methods
        public abstract UniTask Load();
        public abstract UniTask Unload();
    }
}
