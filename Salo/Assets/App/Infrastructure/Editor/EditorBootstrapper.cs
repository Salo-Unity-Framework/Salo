using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/// <summary>
/// Always load ZeroScene (and consequently BootstrapScene) first in Editor Play mode.
/// This ensures that components can access bootstrapped systems. Expecting
/// ZeroScene to be the first entry on the Scene List
/// </summary>
public static class EditorBootstrapper
{
    private static string bootstrapScenePath;
    private static string[] openScenePaths;

    [InitializeOnLoadMethod]
    private static void bootstrap()
    {
        var sceneLoadConfig = SOLoaderEditor.GetUniqueAsset<SceneLoadConfigSO>();
        var sceneLoadRuntimeData = SOLoaderEditor.GetUniqueAsset<SceneLoadRuntimeDataSO>();
        bootstrapScenePath = AssetDatabase.GUIDToAssetPath(sceneLoadConfig.BootstrapScene.AssetGUID);

        // If bootstrapping is disabled, skip everything and load as normal
        if (!BootstrapOnPlayMenuItem.IsBootstrapOnPlayEnabled())
        {
            EditorSceneManager.playModeStartScene = null;
            return;
        }

        // Save all the open scenes. We need to know if ZeroScene or BootstrapScene was open
        // to direct scene load flow. Also, to load the open scenes if needed.
        var openSceneCount = SceneManager.sceneCount;
        openScenePaths = new string[openSceneCount];
        for (int i = 0; i < openSceneCount; i++)
        {
            openScenePaths[i] = SceneManager.GetSceneAt(i).path;
        }

        sceneLoadRuntimeData.CurrentOpenSceneType = getOpenSceneType(sceneLoadConfig);

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
                // Set ZeroScene to load first
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneLoadConfig.ZeroScenePath);
                break;

            case OpenSceneType.Others:
                // Set ZeroScene to load first. Also let EditorBootstrapper load those open scenes in handleFirstSceneLoadRequested
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneLoadConfig.ZeroScenePath);
                SceneLoadEvents.OnFirstSceneLoadRequested += handleFirstSceneLoadRequested;
                break;

            default:
                throw new ArgumentException($"Invalid open scene type: {sceneLoadRuntimeData.CurrentOpenSceneType}");
        }
    }

    private static void handleFirstSceneLoadRequested()
    {
        // NOTE: This should run only if the open scenes did not contain ZeroScene or BootstrapScene

        // Unsubscribe so game restarts will not cause wrong scenes to be loaded here (FirstSceneLoader will take over)
        SceneLoadEvents.OnFirstSceneLoadRequested -= handleFirstSceneLoadRequested;

        Assert.IsTrue(openScenePaths?.Length > 0, "No open scenes detected");

        // NOTE: Assuming only single scenes are open in the Editor. Otherwise
        // we'd need to change the system to load multiple scenes at a time.
        // Also processing Addressables scenes only.
        var sceneReference = getSceneReferenceFromPath(openScenePaths[0]);
        SceneLoadEvents.MajorSceneLoadRequested(sceneReference);
    }

    private static OpenSceneType getOpenSceneType(SceneLoadConfigSO sceneLoadConfig)
    {
        if (openScenePaths.Contains(sceneLoadConfig.ZeroScenePath)) return OpenSceneType.ZeroScene;
        if (openScenePaths.Contains(bootstrapScenePath)) return OpenSceneType.BootstrapScene;
        return OpenSceneType.Others;
    }

    private static SceneReference getSceneReferenceFromPath(string scenePath)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        Assert.IsNotNull(settings);

        foreach (var group in settings.groups)
        {
            foreach (var entry in group.entries)
            {
                if (entry.AssetPath == scenePath)
                {
                    return new SceneReference(entry.guid);
                }
            }
        }

        // The asset was not found. Throw an exception
        throw new ArgumentException($"Addressable asset not found at {scenePath}");
    }
}
