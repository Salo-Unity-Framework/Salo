using UnityEngine;

[CreateAssetMenu(fileName = "DataPersistenceConfig", menuName = "Salo/Config/Data Persistence Config")]
public class DataPersistenceConfigSO : ConfigSOBase
{
    [Tooltip("The current implementation of DataPersistorSOBase")]
    [SerializeField] private DataPersistorSOBase dataPersistor;
    public DataPersistorSOBase DataPersistor => dataPersistor;
}
