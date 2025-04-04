using Cysharp.Threading.Tasks;
using System;
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
    // Handle conversion in "overridden" Save and Load.
    public DateTime PersistedDateTime;
    [Persisted, InspectorReadOnly, SerializeField] private string persistedDateTimeString;

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
        PersistedDateTime = DateTime.Parse(persistedDateTimeString, null, DateTimeStyles.RoundtripKind);
    }
}
