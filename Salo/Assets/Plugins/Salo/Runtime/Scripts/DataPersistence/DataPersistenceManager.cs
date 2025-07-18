using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This bootstrapped system is a tool to handles data persistence for individual
    /// IPersistable classes . It instantiates the correct persistor and uses it when
    /// called to Save or Load by IPersistable classes. Note: Loading as part of the
    /// app flow is done by the PersistedDataLoader loader asset during bootstrap.
    /// </summary>
    public class DataPersistenceManager : StaticInstanceOf<DataPersistenceManager>
    {
        private void OnEnable()
        {
            DataPersistenceEvents.OnResetAllAndSaveRequested += handleResetAllRequested;
        }

        private void OnDisable()
        {
            DataPersistenceEvents.OnResetAllAndSaveRequested -= handleResetAllRequested;
        }

        // Keeping IPersistable as the parameter so the data persistence
        // process is more robust. The instance is used to get the key.
        public void Save(IPersistable persistable, string data)
        {
            var persistor = getPersistor();

            var key = persistable.GetType().Name; // class name

            var isSuccess = persistor.TryWriteString(key, data);
            if (!isSuccess)
            {
                Debug.LogError($"Error writing data for {key}");
            }
        }

        public async UniTask<string> Load(IPersistable persistable)
        {
            var persistor = getPersistor();

            var key = persistable.GetType().Name; // class name
            if (!persistor.HasKey(key))
            {
                Debug.LogWarning($"Persisted data not found for key: {key}");
                return null; // No data saved
            }

            // Get the json string
            var (isSuccess, json) = await persistor.TryReadString(key);
            if (!isSuccess)
            {
                Debug.LogError($"Error reading persisted data with key: {key}");
                return null;
            }

            return json;
        }

        private void handleResetAllRequested()
        {
            // Get the list of persisted runtime data and process the ones that are actually IPersistables
            var runtimeDatas = InfrastructureSOHolder.Instance.DataPersistenceConfig.PersistedRuntimeDatas;
            foreach (var runtimeData in runtimeDatas)
            {
                if (runtimeData is IPersistable persistable)
                {
                    persistable.ResetData();
                    PersistenceHelper.CallConcreteSave(persistable);
                }
            }

            // Get the list of persisted configs and process the ones that are actually IPersistables
            var configs = InfrastructureSOHolder.Instance.DataPersistenceConfig.PersistedConfigs;
            foreach (var config in configs)
            {
                if (config is IPersistable persistable)
                {
                    persistable.ResetData();
                    PersistenceHelper.CallConcreteSave(persistable);
                }
            }
        }

        // Cannot get and store persistor on Awake since this is a StaticInstanceOf too. Which mean it shares
        // the same execution order as other StaticInstanceOf, like InfrastructureSOHolder. We cannot be sure
        // whether this class's Awake or InfrastructureSOHolder's Awake runs first.
        private DataPersistorSOBase getPersistor()
        {
            var persistor = InfrastructureSOHolder.Instance.DataPersistenceConfig.DataPersistor;
            Assert.IsNotNull(persistor);

            return persistor;
        }
    }
}
