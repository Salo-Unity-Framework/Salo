using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This bootstrapped system hold references to the Framework's Infrastructure SO
    /// </summary>
    public class InfrastructureSOHolder : StaticInstanceOf<InfrastructureSOHolder>
    {
        [Header("Configs")]

        [SerializeField] private SceneLoadConfigSO sceneLoadConfig;
        public SceneLoadConfigSO SceneLoadConfig => sceneLoadConfig;

        [SerializeField] private DataPersistenceConfigSO dataPersistenceConfig;
        public DataPersistenceConfigSO DataPersistenceConfig => dataPersistenceConfig;

        [Header("Runtime Data")]

        [SerializeField] private BootstrapRuntimeDataSO bootstrapRuntimeData;
        public BootstrapRuntimeDataSO BootstrapRuntimeData => bootstrapRuntimeData;

        [SerializeField] private SceneLoadRuntimeDataSO sceneLoadRuntimeData;
        public SceneLoadRuntimeDataSO SceneLoadRuntimeData => sceneLoadRuntimeData;
    }
}
