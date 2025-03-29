using System;

public static class SceneLoadEvents
{
    /// <summary>
    /// This is effectively OnMajorSceneLoadRequested with SceneLoadConfig's FirstScene. 
    /// A separate event for FirstScene is needed because EditorBootstrapper needs to
    /// hijack the event to load Editor open scenes instead of the actual FirstScene.
    /// Handled by either 
    /// </summary>
    public static event Action OnFirstSceneLoadRequested;
    public static void FirstSceneLoadRequested()
        => OnFirstSceneLoadRequested?.Invoke();

    public static event Action<SceneReference> OnMajorSceneLoadRequested;
    public static void MajorSceneLoadRequested(SceneReference sceneReference)
        => OnMajorSceneLoadRequested?.Invoke(sceneReference);

    // <summary>
    /// Invoked by SceneLoadManager when starting a scene
    /// load. OnSceneReady will be invoked on completion
    /// </summary>
    public static event Action OnSceneLoadStarted;
    public static void SceneLoadStarted()
        => OnSceneLoadStarted?.Invoke();

    /// <summary>
    /// A Major scene has been loaded by SceneLoadManager, and the scenes
    /// resources have been loaded as part of the custom resource load.
    /// Invoked by SceneLoadManager.
    /// </summary>
    public static event Action OnSceneReady;
    public static void SceneReady()
        => OnSceneReady?.Invoke();
}
