using UnityEngine;
using UnityEngine.Assertions;

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

        // On Editor, ignore the event if EditorBootstrapper will take over
        Assert.IsTrue(AppRuntimeData.Instance.SceneLoadRuntimeData.CurrentOpenSceneType != OpenSceneType.None,
            "Encountered OpenSceneTYpe.None");

        if (AppRuntimeData.Instance.SceneLoadRuntimeData.CurrentOpenSceneType == OpenSceneType.Others)
        {
            return;
        }

#endif

        SceneLoadEvents.MajorSceneLoadRequested(AppConfig.Instance.SceneLoadConfig.FirstScene);
    }
}
