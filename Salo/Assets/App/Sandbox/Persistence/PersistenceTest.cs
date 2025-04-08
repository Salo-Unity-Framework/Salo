using System;
using UnityEngine;

public class PersistenceTest : MonoBehaviour
{
    [SerializeField] private TestPersistedRuntimeDataSO runtimeDataSO;
    [SerializeField] private TestPersistedConfigSO configSO;

    public bool debugSaveTrigger = false;
    public bool debugLogTrigger = false;
    public bool debugResetTrigger = false;
    public bool debugResetAllTrigger = false;

    private void Update()
    {
        // Test calling save on a Config SO
        if (debugSaveTrigger)
        {
            debugSaveTrigger = false;

            // Change values and then call Save
            runtimeDataSO.DefaultRuntimeInt = 10; // will reset to int's default (0) on stopping Play
            runtimeDataSO.PersistedRuntimeInt = 20; // this value will be restored on next game start
            runtimeDataSO.PersistedDateTime = DateTime.Now; // this non-Serializable data will be saved to a string field
            runtimeDataSO.Save();

            // Call the explicit method to change a config value at runtime. This method of setting private
            // fields should be used sparingly since it goes against the architecture design.
            configSO.SetEditableConfigInt(30);
            configSO.Save();
        }

        if (debugLogTrigger)
        {
            debugLogTrigger = false;

            Debug.Log(runtimeDataSO.PersistedDateTime);
        }

        if (debugResetTrigger)
        {
            debugResetTrigger = false;
            runtimeDataSO.ResetData();

            runtimeDataSO.Save();
        }

        if (debugResetAllTrigger)
        {
            debugResetAllTrigger = false;

            DataPersistenceEvents.ResetAllAndSaveRequested();
        }
    }
}
