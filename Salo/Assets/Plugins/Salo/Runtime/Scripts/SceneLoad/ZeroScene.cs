using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// The purpose of the ZeroScene scene is to act as the only scene in
    /// the build list and to load up BootstrapScene. BootstrapScene
    /// cannot be in the build list since it contains Addressables
    /// and therefore must be an Addressable too to avoid
    /// asset duplication.
    /// </summary>
    public class ZeroScene : MonoBehaviour
    {
        [SerializeField] private SceneReference bootstrapScene;

        private void Start()
        {
            bootstrapScene.LoadSceneAsync();
        }
    }
}
