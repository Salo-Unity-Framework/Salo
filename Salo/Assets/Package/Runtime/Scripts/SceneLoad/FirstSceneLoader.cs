using UnityEngine;
using UnityEngine.Assertions;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This bootstrapped system normally handles request for FirstScene load.
    /// An exception is when during Editor Play, EditorBootstrapper hijacks
    /// the event to load the Editor open scene instead of FirstScene.
    /// </summary>
    public class FirstSceneLoader : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneLoadEvents.OnFirstSceneLoadRequested += handleFirstSceneLoadRequested;
        }

        private void OnDisable()
        {
            SceneLoadEvents.OnFirstSceneLoadRequested -= handleFirstSceneLoadRequested;
        }

        private void handleFirstSceneLoadRequested()
        {
#if UNITY_EDITOR
            // On Editor, ignore the event if EditorBootstrapper will take over.
            // Also, if Bootstrap is disabled, CurrentOpenSceneType is not set.
            // Proceed as normal with FirstScene load in such cases.
            if (InfrastructureSOHolder.Instance.SceneLoadRuntimeData.CurrentOpenSceneType == OpenSceneType.Others)
            {
                return;
            }
#endif

            SceneLoadEvents.MajorSceneLoadRequested(InfrastructureSOHolder.Instance.SceneLoadConfig.FirstScene);
        }
    }
}
