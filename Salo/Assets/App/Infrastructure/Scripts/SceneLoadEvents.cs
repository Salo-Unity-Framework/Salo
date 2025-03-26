using System;

public static class SceneLoadEvents
{
    public static event Action OnFirstSceneLoadRequested;
    public static void FirstSceneLoadRequested()
        => OnFirstSceneLoadRequested?.Invoke();

    public static event Action<SceneReference> OnAdditiveLoadRequested;
    public static void AdditiveLoadRequested(SceneReference sceneReference)
        => OnAdditiveLoadRequested?.Invoke(sceneReference);
}
