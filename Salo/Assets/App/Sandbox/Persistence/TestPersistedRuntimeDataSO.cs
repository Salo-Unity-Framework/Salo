using UnityEngine;

[CreateAssetMenu(fileName = "TestPersistedRuntimeData", menuName = "Salo/Sandbox/TestPersistedRuntimeDataSO")]
public class TestPersistedRuntimeDataSO : RuntimeDataSOBase, IPersistable
{
    // Reset-to-default runtime field. Will be reset to its type's default (0 for int) on stopping Editor Play
    // TODO: [EditorResetToDefault]
    public int DefaultRuntimeInt;

    // TODO: [EditorResetToValue]

    // Persisted runtime data. Persisted value will be restored during bootstrap
    [Persisted] public int PersistedRuntimeInt;

    // "Override" of the extension method to run custom code before saving.
    // For example, convert non-Serializable fields to Serializable ones.
    public void Save()
    {
        // TODO: Showcase persisted string to save DateTime data
        Debug.Log("Save on TestPersistedRuntimeDataSO"); // Custom logic for this class before saving
        PersistableExtensions.Save(this); // like base.Save() but for extension methods
    }
}
