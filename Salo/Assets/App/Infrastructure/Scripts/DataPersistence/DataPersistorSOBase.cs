using UnityEngine;

/// <summary>
/// This abstract SO acts like a C# interface for DataPersistor implementations.
/// Subclass SOs should handle the actual disk read and write. The current
/// implementing SO asset should be assigned to DataPersistenceConfig
/// </summary>
public abstract class DataPersistorSOBase : ScriptableObject
{
    /// <summary>
    /// Whether persisted data exists for the given key
    /// </summary>
    /// <returns>true if the key has corresponding data</returns>
    public abstract bool HasKey(string key);

    /// <summary>
    /// Persist the string value with the given key. Should also write to disk.
    /// </summary>
    /// <returns>true if the write was successful</returns>
    public abstract bool TryWriteString(string key, string value);

    /// <summary>
    /// Read the string value with the given key into the "value" out parameter
    /// </summary>
    /// <returns>true if the read was successful</returns>
    public abstract bool TryReadString(string key, out string value);

    /// <summary>
    /// Delete persisted data associated with the given key. Note that this should delete
    /// the data and is not a data reset. It should be considered a debug tool
    /// </summary>
    /// <returns>true if the clear was successful</returns>
    public abstract bool TryClearData(string key);
}
