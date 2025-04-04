using Cysharp.Threading.Tasks;
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
                await persistable.Load();
            }
        }

        // Get the list of persisted configs and process the ones that are actually IPersistables
        var configs = ConfigSOHolder.Instance.DataPersistenceConfig.PersistedConfigs;
        foreach (var config in configs)
        {
            if (config is IPersistable persistable)
            {
                await persistable.Load();
            }
        }
    }


}
