using Cysharp.Threading.Tasks;
using Salo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "TestPersistedRuntimeData", menuName = "Salo/Sandbox/TestPersistedRuntimeDataSO")]
public class TestPersistedRuntimeDataSO : RuntimeDataSOBase, IPersistable
{
    // Normal runtime field. Will be reset to its type's default (0 for int) on stopping Editor Play
    public int DefaultRuntimeInt;

    // Persisted runtime data. "Saved" value will be restored during bootstrap
    [Persisted] public int PersistedRuntimeInt;

    // Non-Serializable type data with Serializable backing field.
    // Handle in "overridden" Save, Load, and ResetData.
    public DateTime PersistedDateTime;
    [Persisted, InspectorReadOnly, SerializeField] private string persistedDateTimeString;

    // Persisted list of floats
    [Persisted] public List<float> PersistedRuntimeFloats;

    // "Override" of the extension method to run custom code before saving.
    // For example, convert non-Serializable fields to Serializable ones.
    public void Save()
    {
        // Convert and assign value of non-Serializable type to Serializable type so it gets saved.
        // ISO 8601 format. Any format works as long as you use the same one when converting back.
        persistedDateTimeString = PersistedDateTime.ToString("o"); 
        PersistableExtensions.Save(this); // like base.Save() but for extension methods
    }

    public async UniTask Load()
    {
        // Load data and then convert and assign back the non-Serializable value
        await PersistableExtensions.Load(this); // like base.Load() but for extension methods

        // Using DateTimeStyles.RoundtripKind since we did PersistedDateTime.ToString("o"); 
        if (!DateTime.TryParse(persistedDateTimeString, null, DateTimeStyles.RoundtripKind, out PersistedDateTime))
        {
            // Parse failed. Assign default
            PersistedDateTime = DateTime.MinValue;
        }
    }
    
    public override void ResetData()
    {
        // This resets the instance's public and private fields (unless they are [NonSerialized]).
        // This includes fields that are not serialized by Unity, so this elminates the need to
        // explicitly propagate the changes to non-Serialized fields (commented out below).
        // This means this override can now be omitted.
        ScriptableObjectHelper.ResetToTypeDefaults(this);

        // DELETED: ScriptableObjectHelper.ResetToTypeDefaults now resets non-Serialized fields too
        //// Propagate the reset data to non-Serialized fields
        //if (!DateTime.TryParse(persistedDateTimeString, null, DateTimeStyles.RoundtripKind, out PersistedDateTime))
        //{
        //    // Parse failed. Assign default
        //    PersistedDateTime = DateTime.MinValue;
        //}
    }
}
