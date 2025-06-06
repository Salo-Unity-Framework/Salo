using UnityEngine;

namespace Salo.Infrastructure
{
    [CreateAssetMenu(fileName = "DataPersistenceConfig", menuName = "Salo/Config/Data Persistence Config")]
    public class DataPersistenceConfigSO : ConfigSOBase
    {
        [Tooltip("The current implementation of DataPersistorSOBase")]
        [SerializeField] private DataPersistorSOBase dataPersistor;
        public DataPersistorSOBase DataPersistor => dataPersistor;

        // PersistedDataLoader will call Load on members implementing IPersistable to
        // load [Persisted] fields. NOTE: Need to check if members are IPersistable

        [Tooltip("RuntimeDataSOBase subclass assets that need to be persisted. Members must be manually added here.")]
        [SerializeField] private RuntimeDataSOBase[] persistedRuntimeDatas;
        public RuntimeDataSOBase[] PersistedRuntimeDatas => persistedRuntimeDatas;

        [Tooltip("ConfigSOBase subclass assets that need to be persisted. Members must be manually added here.")]
        [SerializeField] private ConfigSOBase[] persistedConfigs;
        public ConfigSOBase[] PersistedConfigs => persistedConfigs;
    }
}

