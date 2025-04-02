using UnityEngine;

public class PersistenceTest : MonoBehaviour
{
    public TestConfigSO testConfig;

    public bool debugSaveTrigger = false;

    private void Update()
    {
        // Test calling save on a Config SO
        if (debugSaveTrigger)
        {
            debugSaveTrigger = false;
            testConfig.Save();
        }
    }
}
