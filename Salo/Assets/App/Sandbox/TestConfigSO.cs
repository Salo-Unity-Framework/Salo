using UnityEngine;

[CreateAssetMenu(fileName = "TestConfigSO", menuName = "Scriptable Objects/TestConfigSO")]
public class TestConfigSO : ConfigSOBase, IPersistable
{
    [Persisted, SerializeField] private int test1;
    public int Test1 => test1;

    [SerializeField] private int test2;
    public int Test2;

    public void Save()
    {
        Debug.Log("Save on TestConfigSO"); // Custom logic for this class before saving
        PersistableExtensions.Save(this); // like base.Save() but for extension methods
    }
}
