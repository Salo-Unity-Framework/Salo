using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This bootstrapped system hold references to the
    /// various ConfigSOBase subclasses in the project
    /// </summary>
    public class ConfigSOHolder : StaticInstanceOf<ConfigSOHolder>
    {
        [SerializeField] private SceneLoadConfigSO sceneLoadConfig;
        public SceneLoadConfigSO SceneLoadConfig => sceneLoadConfig;

        [SerializeField] private DataPersistenceConfigSO dataPersistenceConfig;
        public DataPersistenceConfigSO DataPersistenceConfig => dataPersistenceConfig;
    }
}
