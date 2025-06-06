using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This base class registers itself to SceneLoadRuntimeData for its FadeOut method to be called
    /// when SceneLoadManager attempts to unload a major scene. Subclasses should implement the 
    /// actual scene-specific or project-specific fades. Note that fading in is not included
    /// in the base class since it does not affect the scene loading/unloading flow.
    /// </summary>
    public abstract class SceneFaderBase : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            var sceneLoadRuntimeData = InfrastructureSOHolder.Instance.SceneLoadRuntimeData;

            // Assign self so SceneLoadManager can call FadeOut
            if (null != sceneLoadRuntimeData.CurrentSceneFader)
            {
                Debug.LogWarning("sceneLoadRuntimeData.CurrentSceneFader was not null. Replacing fader");
            }

            sceneLoadRuntimeData.CurrentSceneFader = this;
        }

        protected virtual void OnDisable()
        {
            InfrastructureSOHolder.Instance.SceneLoadRuntimeData.CurrentSceneFader = null;
        }

        /// <summary>
        /// Should be implemented as an async method. SceneLoadManager
        /// awaits this before unloading the scene
        /// </summary>
        public abstract UniTask FadeOut();
    }
}
