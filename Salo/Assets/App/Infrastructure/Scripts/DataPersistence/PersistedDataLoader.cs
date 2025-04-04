using Cysharp.Threading.Tasks;
using System.Reflection;
using UnityEngine;

public class PersistedDataLoader : BootstrapResourceLoaderBase
{
    public async override UniTask Load()
    {
        Debug.Log("Loading persted data");

        // Get the list of persisted runtime data and process the ones that are actually IPersistables
        var runtimeDatas = ConfigSOHolder.Instance.DataPersistenceConfig.PersistedRuntimeDatas;
        foreach (var runtimeData in runtimeDatas)
        {
            if (runtimeData is IPersistable persistable)
            {
                await callMethodOnIPersistable(persistable, nameof(PersistableExtensions.Load));
            }
        }

        // Get the list of persisted configs and process the ones that are actually IPersistables
        var configs = ConfigSOHolder.Instance.DataPersistenceConfig.PersistedConfigs;
        foreach (var config in configs)
        {
            if (config is IPersistable persistable)
            {
                await callMethodOnIPersistable(persistable, nameof(PersistableExtensions.Load));
            }
        }
    }

    // Since persistable is cast as IPersistable, calling Load on it will call the extension method instead of
    // the method on the concrete implementation. Make sure to call the concrete class's method if it exists.
    private static async UniTask callMethodOnIPersistable(IPersistable persistable, string methodName)
    {
        var type = persistable.GetType(); // The concrete type
        var methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        if (null != methodInfo)
        {
            // If found, call the "override" method on the concrete type
            var result = methodInfo.Invoke(persistable, null);
            if (result is UniTask task)
            {
                await task;
            }
        }
        {
            // Method not found declared on the concrete implementing task. Call the default extension method
            await persistable.Load();
        }
    }
}
