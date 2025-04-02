using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This bootstrapped system handles data persistence. It instantiates the correct
/// persistor and uses it when called to Save or Load by IPersistable classes.
/// </summary>
public class DataPersistenceManager : StaticInstanceOf<DataPersistenceManager>
{
    private DataPersistorSOBase persistor; // Store the current implementation

    protected override void Awake()
    {
        base.Awake();

        // Get the assigned persistor implementation
        persistor = AppConfig.Instance.DataPersistenceConfig.DataPersistor;
        Assert.IsNotNull(persistor);
    }

    // TODO: Listen for requests to save all, clear all etc

    // Keeping IPersistable as the parameter so the data persistence
    // process is more robust. The instance is used to get the key.
    public void Save(IPersistable persistable, string data)
    {
        var key = persistable.GetType().Name; // class name

        var isSuccess = persistor.TryWriteString(key, data);
        if (!isSuccess)
        {
            Debug.LogError($"Error writing data for {key}");
        }
    }
}
