using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Base class for Scene resource loaders. Note that base classes in a loaded scene
/// will add themselves to SceneLoadRuntimeData even if the component is disabled.
/// </summary>
public abstract class SceneResourceLoaderBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        AppRuntimeData.Instance.SceneLoadRuntimeData.SceneResourceLoaders.Add(this);
    }

    protected virtual void OnDestroy()
    {
        AppRuntimeData.Instance.SceneLoadRuntimeData.SceneResourceLoaders.Remove(this);
    }

    // Should be implemented as async methods
    public abstract UniTask Load();
    public abstract UniTask Unload();
}
