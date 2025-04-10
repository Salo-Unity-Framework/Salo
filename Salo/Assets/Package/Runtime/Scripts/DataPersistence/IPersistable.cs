using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This is meant to be implemented by RuntimeDataSOBase (and rarely ConfigSOBase - and other types too)
/// subclasses that have [Persisted] fields. The extension methods on IPersistable allow us to add
/// extended functionality to classes without affecting inheritance. C# 8+ default interface
/// methods are not yet supported by IL2CPP.
/// </summary>
public interface IPersistable
{
    /// <summary>
    /// Reset fields to their original values. The reset logic will depend on the type of
    /// implementation so this cannot be a default extension method like Save and Load.
    /// For Runtime Data SOs, this should simply be a call to
    /// ScriptableObjectHelper.ResetToTypeDefaults.
    /// </summary>
    public void ResetData();
}

public static class PersistableExtensions
{
    // NOTE: Data types that cannot be serialized can instead be converted and
    // saved to a Serializable field on an overridden Save(). This should be
    // done before calling base.Save(). Similarly on load, those fields can
    // be converted back on overridden Load() after calling base.Load().

    /// <summary>
    /// Save should be called explicitly whenever data on the instance needs to be saved.
    /// It is not called implicitly as part of any data persistence process.
    /// </summary>
    public static void Save(this IPersistable persistable)
    {
        // Get an object (and then JSON) with only [Persisted] fields
        var persistedObject = PersistenceHelper.GetPersistedObject(persistable);
        var json = JsonUtility.ToJson(persistedObject);

        DataPersistenceManager.Instance.Save(persistable, json);
    }

    /// <summary>
    /// Load is called on IPersistables assigned (manually) to DataPersistenceConfig.PersistedConfigs
    /// by PersistedDataLoader as part of the bootstrap flow.
    /// </summary>
    public static async UniTask Load(this IPersistable persistable)
    {
        var json = await DataPersistenceManager.Instance.Load(persistable);

        if (string.IsNullOrEmpty(json)) return; // Error/Warning already logged by DataPersistenceManager

        JsonUtility.FromJsonOverwrite(json, persistable);
    }
}
