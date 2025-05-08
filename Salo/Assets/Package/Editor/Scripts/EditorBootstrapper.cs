using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Salo.Infrastructure.EditorExtensions
{
    /// <summary>
    /// Always load ZeroScene (and consequently BootstrapScene) first in Editor Play mode.
    /// This ensures that components can access bootstrapped systems. Expecting
    /// ZeroScene to be the first entry on the Scene List
    /// </summary>
    public static class EditorBootstrapper
    {
        [InitializeOnLoadMethod]
        private static void initializeOnLoad()
        {
            EditorApplication.playModeStateChanged -= handlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += handlePlayModeStateChanged;
        }

        private static void handlePlayModeStateChanged(PlayModeStateChange state)
        {
            // Bootstrap in two stages on Editor Play
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    processExitingEditMode();
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    processEnteringPlayMode();
                    break;
            }
        
        }

        // Runs early on starting Editor Play. Note that Debug logs and class fields set here are cleared
        private static void processExitingEditMode()
        {
            var sceneLoadConfig = SOLoaderEditor.GetUniqueAsset<SceneLoadConfigSO>();
            var sceneLoadRuntimeData = SOLoaderEditor.GetUniqueAsset<SceneLoadRuntimeDataSO>();

            // If bootstrapping is disabled, skip everything and load as normal
            if (!BootstrapOnPlayMenuItem.IsBootstrapOnPlayEnabled())
            {
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            // Save changes to open scenes
            EditorSceneManager.SaveOpenScenes();

            // Save all the open scenes. We need to know if ZeroScene or BootstrapScene was
            // open to direct scene load flow. Also, to load the open scenes if needed.
            // Save to SO so the info persists through to processEnteringPlayMode.
            var openSceneCount = SceneManager.sceneCount;
            sceneLoadRuntimeData.OpenScenePaths = new string[openSceneCount];
            for (int i = 0; i < openSceneCount; i++)
            {
                sceneLoadRuntimeData.OpenScenePaths[i] = SceneManager.GetSceneAt(i).path;
            }

            SceneHierarchyPerserver.SaveHierarchyState(); // saves to sceneLoadRuntimeData

            // Save to SO so the info persists through to processEnteringPlayMode
            sceneLoadRuntimeData.CurrentOpenSceneType = getOpenSceneType(sceneLoadConfig, sceneLoadRuntimeData.OpenScenePaths);

            // If the active scene is the zero scene, there is nothing to change.
            // ZeroScene should run normally and will load BootstrapScene.
            // Note that this is run on InitializeOnLoadMethod and will
            // take effect on the next Editor Play only.
            switch (sceneLoadRuntimeData.CurrentOpenSceneType)
            {
                case OpenSceneType.ZeroScene:
                    EditorSceneManager.playModeStartScene = null;
                    break;

                case OpenSceneType.BootstrapScene:
                case OpenSceneType.Others:
                    // Set ZeroScene to load first
                    EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneHelperEditor.ZeroScenePath);
                    break;

                default:
                    throw new ArgumentException($"Invalid open scene type: {sceneLoadRuntimeData.CurrentOpenSceneType}");
            }
        }

        // Runs later on starting Editor Play. Info needed here from processExitingEditMode
        // should be assigned to SceneLoadRuntimeDataSO fields so they persist. Also,
        // event subscriptions should be done here instead of processExitingEditMode.
        private static void processEnteringPlayMode()
        {
            // Restore hierarchy states on scene load
            SceneLoadEvents.OnMajorSceneLoaded -= handleMajorSceneLoaded;
            SceneLoadEvents.OnMajorSceneLoaded += handleMajorSceneLoaded;

            var sceneLoadRuntimeData = SOLoaderEditor.GetUniqueAsset<SceneLoadRuntimeDataSO>();
            if (sceneLoadRuntimeData.CurrentOpenSceneType == OpenSceneType.Others)
            {
                SceneLoadEvents.OnFirstSceneLoadRequested += handleFirstSceneLoadRequested;
            }
        }

        private static void handleMajorSceneLoaded(Scene _)
        {
            // Restore scene hierarachy expanded states and selection. This will run on loading completion
            // of either FirstScene (FirstSceneLoader) or the Editor open scenes (EditorBootstrapper).

            SceneLoadEvents.OnMajorSceneLoaded -= handleMajorSceneLoaded;
            SceneHierarchyPerserver.RestoreHierarchyState();
        }

        private static void handleFirstSceneLoadRequested()
        {
            // NOTE: This should run only if the open scenes did not contain ZeroScene or BootstrapScene

            // Unsubscribe so game restarts will not cause wrong scenes to be loaded here (FirstSceneLoader will take over)
            SceneLoadEvents.OnFirstSceneLoadRequested -= handleFirstSceneLoadRequested;

            var sceneLoadRuntimeData = SOLoaderEditor.GetUniqueAsset<SceneLoadRuntimeDataSO>();
            Assert.IsTrue(sceneLoadRuntimeData.OpenScenePaths?.Length > 0, "No open scenes detected");

            // NOTE: Assuming only single scenes are open in the Editor. Otherwise
            // we'd need to change the system to load multiple scenes at a time.
            // Also processing Addressables scenes only.
            var sceneReference = getSceneReferenceFromPath(sceneLoadRuntimeData.OpenScenePaths[0]);
            SceneLoadEvents.MajorSceneLoadRequested(sceneReference);
        }

        private static OpenSceneType getOpenSceneType(SceneLoadConfigSO sceneLoadConfig, string[] openScenePaths)
        {
            var bootstrapScenePath = AssetDatabase.GUIDToAssetPath(sceneLoadConfig.BootstrapScene.AssetGUID);

            if (openScenePaths.Contains(SceneHelperEditor.ZeroScenePath)) return OpenSceneType.ZeroScene;
            if (openScenePaths.Contains(bootstrapScenePath)) return OpenSceneType.BootstrapScene;
            return OpenSceneType.Others;
        }

        private static SceneReference getSceneReferenceFromPath(string scenePath)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            Assert.IsNotNull(settings);

            Assert.IsTrue(AssetDatabase.AssetPathExists(scenePath), $"Addressable asset not found at {scenePath}");
            var sceneGuid = AssetDatabase.AssetPathToGUID(scenePath);

            return new SceneReference(sceneGuid);
        }
    }
}
