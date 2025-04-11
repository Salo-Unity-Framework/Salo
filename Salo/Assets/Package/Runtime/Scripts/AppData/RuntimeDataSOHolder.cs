using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This bootstrapped system hold references to the various
    /// RuntimeDataSOBase subclasses in the project
    /// </summary>
    public class RuntimeDataSOHolder : StaticInstanceOf<RuntimeDataSOHolder>
    {
        [SerializeField] private BootstrapRuntimeDataSO bootstrapRuntimeData;
        public BootstrapRuntimeDataSO BootstrapRuntimeData => bootstrapRuntimeData;

        [SerializeField] private SceneLoadRuntimeDataSO sceneLoadRuntimeData;
        public SceneLoadRuntimeDataSO SceneLoadRuntimeData => sceneLoadRuntimeData;
    }
}
