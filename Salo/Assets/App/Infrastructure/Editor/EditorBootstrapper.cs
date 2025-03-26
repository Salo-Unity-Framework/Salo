using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/// <summary>
/// Always load the Bootstrap scene first in Editor Play mode. This ensures
/// that components can access the bootstrap components during their
/// initialization. Expecting BootstrapScene at scene index 0.
/// </summary>
public static class EditorBootstrapper
{
    private static SceneLoadConfigSO sceneLoadConfig;
    private static string bootstrapScenePath;
    private static string[] openScenePaths;

    [InitializeOnLoadMethod]
    private static void bootstrap()
    {
        sceneLoadConfig = SOLoaderEditor.GetUniqueAsset<SceneLoadConfigSO>();
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

        var openSceneType = getOpenSceneType(openScenePaths);

        // If the active scene is the zero scene, there is nothing to change.
        // ZeroScene should run normally and will load BootstrapScene.
        // Note that this is run on InitializeOnLoadMethod and will
        // take effect on the next Editor Play only.
        if (openSceneType == OpenSceneType.ZeroScene)
        {
            EditorSceneManager.playModeStartScene = null;
            return;
        }

        // Set Bootstrap scene to load first
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(bootstrapScenePath);

        // If the open scenes were ZeroScene or BootstrapScene, FirstSceneLoader will.
        // take over as normal Otherwise EditorBootstrapper will load the open scenes.
        if (openSceneType == OpenSceneType.Others)
        {
            SceneLoadEvents.OnFirstSceneLoadRequested += handleFirstSceneLoadRequested;
        }
    }

    private static void handleFirstSceneLoadRequested()
    {
        // NOTE: This should run only if the open scenes did not contain ZeroScene or BootstrapScene

        // Unsubscribe so game restarts will not cause wrong scenes to be loaded here (FirstSceneLoader will take over)
        SceneLoadEvents.OnFirstSceneLoadRequested -= handleFirstSceneLoadRequested;

        Assert.IsNotNull(openScenePaths);
        Assert.IsTrue(openScenePaths.Length > 0);

        // TODO: Fade out

        // NOTE: Assuming only single scenes are open in the Editor. Otherwise
        // we'd need to change the system to load multiple scenes at a time.
        // Also processing Addressables scenes only.
        var sceneReference = getSceneReferenceFromPath(openScenePaths[0]);
        SceneLoadEvents.AdditiveLoadRequested(sceneReference);
    }

    private static OpenSceneType getOpenSceneType(string[] openScenePaths)
    {
        if (openScenePaths.Contains(sceneLoadConfig.ZeroScenePath)) return OpenSceneType.ZeroScene;
        else if (openScenePaths.Contains(bootstrapScenePath)) return OpenSceneType.BootstrapScene;
        else return OpenSceneType.Others;
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

    private enum OpenSceneType
    {
        None,
        ZeroScene,
        BootstrapScene,
        Others,
    }
}
