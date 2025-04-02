using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This bootstrapped system loads and unloads bootstrapped resources. THe load method is run by
/// AppBootstrap during the bootstrap process. Loaders in BootstrapSCene will register
/// themselves during BootstrapScene load to be used by this class.
public class BootstrapResourceManager : StaticInstanceOf<BootstrapResourceManager>
{
    public async UniTask Load()
    {
        var loaders = RuntimeDataSOHolder.Instance.BootstrapRuntimeData.BootstrapResourceLoaders;
        var tasks = new UniTask[loaders.Count];

        for (int i = 0; i < tasks.Length; i++)
        {
            if (null == loaders[i]) continue; // Avoid invalid tasks
            tasks[i] = loaders[i].Load();
        }

        await UniTask.WhenAll(tasks);

        Debug.Log($"Bootstrap resource loading complete. Processed {tasks.Length}/{loaders.Count} loaders");
    }
}
