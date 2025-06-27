using UnityEngine;

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
            // If Bootstrap is disabled SessionState's OPEN_SCENE_TYPE is not assigned.
            // In that case, proceed as normal with FirstScene load. Else on Editor,
            // ignore this if EditorBootstrapper will take over (Others scene type).

            var openSceneTypeString = UnityEditor.SessionState.GetString(SceneLoadRuntimeDataSO.OPEN_SCENE_TYPE_KEY, null);

            if (string.IsNullOrEmpty(openSceneTypeString))
            {
                // SessionState's OPEN_SCENE_TYPE is not assigned. Bootstrap is disabled. Proceed as normal
            }
            else
            {
                // Else EditorBootatrapper will take over on OpenSceneType of 'Others'. Ignore this event
                var openSceneType = System.Enum.Parse<OpenSceneType>(openSceneTypeString);
                if (openSceneType == OpenSceneType.Others) return;
            }
#endif

            SceneLoadEvents.MajorSceneLoadRequested(InfrastructureSOHolder.Instance.SceneLoadConfig.FirstScene);
        }
    }
}
