using Salo.Infrastructure;
using UnityEngine;

[CreateAssetMenu(fileName = "TestPersistedConfig", menuName = "Salo/Sandbox/TestPersistedConfigSO")]
public class TestPersistedConfigSO : ConfigSOBase, IPersistable
{
    // Normal config data. The value is editable on the asset during
    // edit-time but is not expected to be changed during runtime.
    [SerializeField] private int normalConfigInt;
    public int NormalConfigInt => normalConfigInt;

    // Like normal config data, but with a method to change the private field's value
    // at runtime. This goes against the architecture design and should be used
    // sparingly. Use persisted RuntimeData fields instead when possible.
    // Note that the private backing field is [Persisted].
    [Persisted, SerializeField] private int editableConfigInt;
    public int EditableConfigInt => editableConfigInt;

    // Explicit setter to change a private config value. Save needs to be
    // called to persist the value (make sure it is a [Persisted] field).
    public void SetEditableConfigInt(int value) => editableConfigInt = value;

    public void ResetData()
    {
        // Generally there should be nothing to reset for config data
    }
}
