using System;
using UnityEngine.SceneManagement;

namespace Salo.Infrastructure
{
    public static class SceneLoadEvents
    {
        /// <summary>
        /// This is effectively OnMajorSceneLoadRequested with SceneLoadConfig's FirstScene. 
        /// A separate event for FirstScene is needed because EditorBootstrapper needs to
        /// hijack the event to load Editor open scenes instead of the actual FirstScene.
        /// Handled by either FirstSceneLoader or EditorBootstrapper.
        /// </summary>
        public static event Action OnFirstSceneLoadRequested;
        public static void FirstSceneLoadRequested()
            => OnFirstSceneLoadRequested?.Invoke();

        /// <summary>
        /// Request to load a Major scene. Handled by SceneLoadManager
        /// </summary>
        public static event Action<SceneReference> OnMajorSceneLoadRequested;
        public static void MajorSceneLoadRequested(SceneReference sceneReference)
            => OnMajorSceneLoadRequested?.Invoke(sceneReference);

        /// <summary>
        /// Request to reload the currently loaded Major scene. Handled by SceneLoadManager.
        /// </summary>
        public static event Action OnReloadRequested;
        public static void ReloadRequested()
            => OnReloadRequested?.Invoke();

        /// <summary>
        /// Request to reload the scene assigned to SceneLoadConfig.TitleScene.
        /// Handled by SceneLoadManager.
        /// </summary>
        public static event Action OnTitleSceneLoadRequested;
        public static void TitleSceneLoadRequested()
            => OnTitleSceneLoadRequested?.Invoke();

        /// <summary>
        /// Notify start of scene fadeout. Invoked by SceneLoadManager
        /// </summary>
        public static event Action OnFadeOutStarted;
        public static void FadeOutStarted()
            => OnFadeOutStarted?.Invoke();

        // <summary>
        /// Invoked by SceneLoadManager when starting a scene
        /// load. OnSceneReady will be invoked on completion
        /// </summary>
        public static event Action OnSceneLoadStarted;
        public static void SceneLoadStarted()
            => OnSceneLoadStarted?.Invoke();

        /// <summary>
        /// Notify that Unity has loaded a Major scene. This is invoked
        /// before OnSceneReady, before the SceneResourceLoad.
        /// Invoked by SceneLoadManager.
        /// </summary>
        public static event Action OnMajorSceneLoaded;
        public static void MajorSceneLoaded()
            => OnMajorSceneLoaded?.Invoke();

        /// <summary>
        /// A Major scene has been loaded by SceneLoadManager, and the scenes
        /// resources have been loaded as part of the custom resource load.
        /// Invoked by SceneLoadManager.
        /// </summary>
        public static event Action OnSceneReady;
        public static void SceneReady()
            => OnSceneReady?.Invoke();

        /// <summary>
        /// A Major scene load process has failed. Invoked by SceneLoadManager
        /// </summary>
        public static event Action OnMajorSceneLoadFailed;
        public static void MajorSceneLoadFailed()
            => OnMajorSceneLoadFailed?.Invoke();
    }
}
