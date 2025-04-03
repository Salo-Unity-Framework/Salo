using Cysharp.Threading.Tasks;
using UnityEngine;

public class PersistedDataLoader : BootstrapResourceLoaderBase
{
    public async override UniTask Load()
    {
        Debug.Log("Loading persted data");

        // Get the list of persisted configs and process the ones that are actually IPersistable
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
