using UnityEngine;

/// <summary>
/// Base class for Config SOs. Subclass SOs should contain data
/// that are static (mostly) during runtime, as opposed to
/// Runtime SOs that will contain runtime data
/// </summary>
public abstract class ConfigSOBase : ScriptableObject
{
    /// <summary>
    /// Save should be called explicitly whenever data on the SO needs to be saved.
    /// It is not called implicitly as part of any data persistence process.
    /// </summary>
    public virtual void Save()
    {
        // Get an object (and then JSON) with only [Persisted] fields
        var persistedObject = PersistenceHelper.GetPersistedObject(this);
        var json = JsonUtility.ToJson(persistedObject);

        DataPersistenceManager.Instance.Save(this, json);
    }

    public virtual void Load()
    {
        // TODO
    }

    // NOTE: Data types that cannot be serialized can instead be converted and
    // saved to a Serializable field on an overridden Save(). This should be
    // done before calling base.Save(). Similarly on load, those fields can
    // be converted back on overridden Load() after calling base.Load().
}
