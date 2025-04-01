using UnityEngine;
using UnityEngine.Assertions;

public class DataPersistenceManager : StaticInstanceOf<DataPersistenceManager>
{
    private DataPersistorSOBase persistor;

    protected override void Awake()
    {
        base.Awake();

        // Get the assigned persistor implementation
        persistor = AppConfig.Instance.DataPersistenceConfig.DataPersistor;
        Assert.IsNotNull(persistor);
    }

    // TODO: Listen for requests to save all, clear all etc

    // Keeping ConfigSOBase as the parameter so the data persistence
    // process is more robust. This is used to get the key.
    public void Save(ConfigSOBase configSO, string data)
    {
        var key = configSO.GetType().Name;

        var isSuccess = persistor.TryWriteString(key, data);
        if (!isSuccess)
        {
            Debug.LogError($"Error writing data for {key}");
        }
    }
}
