using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Salo.Infrastructure
{
    /// <summary>
    /// This bootstrapped system handles scene loads
    /// </summary>
    public class SceneLoadManager : MonoBehaviour
    {
        private bool isLoadingMajorScene = false;

        private SceneInstance loadedMajorSceneInstance;
        private SceneReference loadedMajorSceneReference;

        private void OnEnable()
        {
            SceneLoadEvents.OnMajorSceneLoadRequested += handleMajorSceneLoadRequested;
            SceneLoadEvents.OnReloadRequested += handleReloadRequested;
            SceneLoadEvents.OnTitleSceneLoadRequested += handleTitleSceneLoadRequested;
        }

        private void OnDisable()
        {
            SceneLoadEvents.OnMajorSceneLoadRequested -= handleMajorSceneLoadRequested;
            SceneLoadEvents.OnReloadRequested -= handleReloadRequested;
            SceneLoadEvents.OnTitleSceneLoadRequested -= handleTitleSceneLoadRequested;
        }

        private async void handleMajorSceneLoadRequested(SceneReference sceneReference)
        {
            if (isLoadingMajorScene)
            {
                Debug.LogError($"Attempting to load another Major scene while already loading one");
                return;
            }

            if (!sceneReference.RuntimeKeyIsValid())
            {
                Debug.LogError("Attempted to load unassigned SceneReference");
                return;
            }

            isLoadingMajorScene = true;

            // Fade out if fader is assigned
            var sceneLoadRuntimeData = InfrastructureSOHolder.Instance.SceneLoadRuntimeData;
            if (null != sceneLoadRuntimeData.CurrentSceneFader)
            {
                SceneLoadEvents.FadeOutStarted();
                await sceneLoadRuntimeData.CurrentSceneFader.FadeOut();
            }

            SceneLoadEvents.SceneLoadStarted();

            // Unload current major scene and its resources if any
            await unloadCurrentSceneAndResources();

            var loadedScene = await loadMajorScene(sceneReference);
            SceneLoadEvents.MajorSceneLoaded(loadedScene);

            await SceneResourceManager.Instance.Load();

            isLoadingMajorScene = false;

            SceneLoadEvents.SceneReady();

            // Note: SceneFaders may implement their own fade-in logic. If they do, they
            // should start the logic - it will not be started from here as there isn't
            // anything that waits for the fade-in to finish here (yet).
        }

        private void handleReloadRequested()
        {
            Assert.IsNotNull(loadedMajorSceneReference);
            SceneLoadEvents.MajorSceneLoadRequested(loadedMajorSceneReference);
        }

        private void handleTitleSceneLoadRequested()
        {
            var sceneReference = InfrastructureSOHolder.Instance.SceneLoadConfig.TitleScene;
            SceneLoadEvents.MajorSceneLoadRequested(sceneReference);
        }

        public async UniTask unloadCurrentSceneAndResources()
        {
            await SceneResourceManager.Instance.Unload();

            if (!loadedMajorSceneInstance.Scene.IsValid())
            {
                // loadedMajorSceneInstance is not assigned until the SceneLoadManager loads
                // the first Majopr scene. It will be invalid on the first Major scene load.
                //Debug.LogError("Attempted to unload invalid Scene");
                return;
            }

            var unloadedSceneName = loadedMajorSceneInstance.Scene.name; // Store while still valid
            var handle = Addressables.UnloadSceneAsync(loadedMajorSceneInstance, autoReleaseHandle: false);
            await handle.Task.AsUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Scene unloaded: {unloadedSceneName}");
            }
            else if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to unload scene: {unloadedSceneName}");
            }

            Addressables.Release(handle);
        }

        private async UniTask<Scene> loadMajorScene(SceneReference sceneReference)
        {
            // Load the scene without activating to avoid calls to Start
            // before the scene is set as the active scene.
            var handle = sceneReference.LoadSceneAsync(LoadSceneMode.Additive, activateOnLoad: false);
            await handle.Task.AsUniTask();

            // Process load result
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedMajorSceneInstance = handle.Result;
                loadedMajorSceneReference = sceneReference;

                // Manually activate the scene since it was loaded using activateOnLoad = false
                // HACK: Setting activateOnLoad = false and manually activating the scene here
                // somehow fixes the issue with Start being called on the loaded scene's
                // script before it was set active - this resulted in objects being
                // instantiated in other scenes if instantiated in Start.
                await handle.Result.ActivateAsync();

                Debug.Log($"Major scene loaded: {loadedMajorSceneInstance.Scene.name}");

                // Note: Loaded major scene. Set it active
                SceneManager.SetActiveScene(loadedMajorSceneInstance.Scene);
            }
            else if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to load major scene. GUID: {sceneReference.AssetGUID}");
            }

            return handle.Result.Scene;
        }
    }
}
