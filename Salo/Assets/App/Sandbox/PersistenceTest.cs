using UnityEngine;

public class PersistenceTest : MonoBehaviour
{
    public TestConfigSO testConfig;

    public bool debugSaveTrigger = false;

    private void Update()
    {
        if (debugSaveTrigger)
        {
            debugSaveTrigger = false;
            testConfig.temp1++;
            testConfig.temp2++;
            testConfig.Save();
        }
    }
}
