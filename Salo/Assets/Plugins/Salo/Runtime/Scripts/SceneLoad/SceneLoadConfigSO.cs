using UnityEngine;

namespace Salo.Infrastructure
{
    [CreateAssetMenu(fileName = "SceneLoadConfig", menuName = "Salo/Config/Scene Load Config")]
    public class SceneLoadConfigSO : ConfigSOBase
    {
        [Tooltip("The bootstrap scene")]
        [SerializeField] private SceneReference bootstrapScene;
        public SceneReference BootstrapScene => bootstrapScene;

        [Tooltip("The first scene to load after bootstrapping")]
        [SerializeField] private SceneReference firstScene;
        public SceneReference FirstScene => firstScene;

        [Header("(Optional)")]

        [Tooltip("The scene to load when calling SceneLoadEvents.TitleSceneLoadRequested")]
        [SerializeField] private SceneReference titleScene;
        public SceneReference TitleScene => titleScene;
    }
}
