using System.Dynamic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestConfigSO", menuName = "Scriptable Objects/TestConfigSO")]
public class TestConfigSO : ConfigSOBase
{
    [Persisted] public int temp1;
    public int temp2;
}
