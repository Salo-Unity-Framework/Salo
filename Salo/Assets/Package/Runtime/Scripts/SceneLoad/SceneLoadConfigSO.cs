using UnityEngine;
using UnityEngine.SceneManagement;

namespace Salo.Infrastructure
{
    [CreateAssetMenu(fileName = "SceneLoadConfig", menuName = "Salo/Config/Scene Load Config")]
    public class SceneLoadConfigSO : ConfigSOBase
    {
        [Tooltip("The path to ZeroScene. Read from the Scene List, assuming it is the first scene")]
        public string ZeroScenePath => SceneUtility.GetScenePathByBuildIndex(0);

        [Tooltip("An Addressable reference to BootstrapScene")]
        [SerializeField] private SceneReference bootstrapScene;
        public SceneReference BootstrapScene => bootstrapScene;

        [Tooltip("The first scene to load after bootstrapping")]
        [SerializeField] private SceneReference firstScene;
        public SceneReference FirstScene => firstScene;
    }
}
