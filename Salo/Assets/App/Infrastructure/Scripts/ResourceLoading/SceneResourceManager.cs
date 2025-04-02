using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This bootstrapped system loads and unloads resources for Major scenes. It is used
/// by SceneLoadManager. ResourceLoaders will register themselves during scene load
/// to be used by this class. This is different from BootstrapResourceLoader,
/// which is meant only for loading during bootstrap.
/// </summary>
public class SceneResourceManager : StaticInstanceOf<SceneResourceManager>
{
    // Run the load method on registered loaders. Called by SceneLoadManager
    public async UniTask Load()
    {
        // Note: Loaders register themselves to the SceneLoadRuntimeData asset's ResourceLoaders on Awake

        var loaders = RuntimeDataSOHolder.Instance.SceneLoadRuntimeData.SceneResourceLoaders;
        var tasks = new UniTask[loaders.Count];

        for (int i = 0; i < tasks.Length; i++)
        {
            if (null == loaders[i]) continue; // Avoid invalid tasks
            tasks[i] = loaders[i].Load();
        }

        await UniTask.WhenAll(tasks);

        Debug.Log($"Scene resource loading complete. Processed {tasks.Length}/{loaders.Count} loaders");
    }

    // Run the unload method on registered loaders. Called by SceneLoadManager
    public async UniTask Unload()
    {
        var loaders = RuntimeDataSOHolder.Instance.SceneLoadRuntimeData.SceneResourceLoaders;
        var tasks = new UniTask[loaders.Count];

        for (int i = 0; i < tasks.Length; i++)
        {
            if (null == loaders[i]) continue;
            tasks[i] = loaders[i].Unload();
        }

        await UniTask.WhenAll(tasks);

        // NOTE: Loaders will unregister themselves on their OnDestroy when their scene unloads

        Debug.Log($"Scene resource unloading complete. Processed {tasks.Length}/{loaders.Count} loaders");
    }
}
