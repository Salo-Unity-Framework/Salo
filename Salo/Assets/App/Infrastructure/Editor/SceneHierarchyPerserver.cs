using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is used by EditorBootstrapper to save and restore
/// the scene objects' expnaded and selected states
/// </summary>
public static class SceneHierarchyPerserver
{
    // Method on Unity's internal class
    private static MethodInfo setExpandedMethodHolder;
    private static MethodInfo setExpandedMethod
    {
        get
        {
            if (null == setExpandedMethodHolder)
            {
                setExpandedMethodHolder = hierarchyWindowType.GetMethod("SetExpanded", BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.IsNotNull(setExpandedMethodHolder, "SetExpanded method not found on UnityEditor.SceneHierarchyWindow.");
            }

            return setExpandedMethodHolder;
        }
    }

    // Method on Unity's internal class
    private static MethodInfo getExpandedIDsMethodHolder;
    private static MethodInfo getExpandedIDsMethod
    {
        get
        {
            if (null == getExpandedIDsMethodHolder)
            {
                getExpandedIDsMethodHolder = hierarchyWindowType.GetMethod("GetExpandedIDs", BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.IsNotNull(getExpandedIDsMethodHolder, "GetExpandedIDs method not found on UnityEditor.SceneHierarchyWindow.");
            }

            return getExpandedIDsMethodHolder;
        }
    }

    private static Type hierarchyWindowType => typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");

    private static EditorWindow hierarchyWindowHolder;
    private static EditorWindow hierarchyWindow
    {
        get
        {
            if (null == hierarchyWindowHolder)
            {
                hierarchyWindowHolder = Resources.FindObjectsOfTypeAll(hierarchyWindowType).FirstOrDefault() as EditorWindow;
                Assert.IsNotNull(hierarchyWindowHolder, "SceneHierarchyWindow instance not found.");
            }

            return hierarchyWindowHolder;
        }
    }

    public static void SaveHierarchyState()
    {
        var sceneLoadRuntimeData = SOLoaderEditor.GetUniqueAsset<SceneLoadRuntimeDataSO>();

        // Save expanded objects
        sceneLoadRuntimeData.ExpandedObjectPaths = getExpandedPaths();

        // Save selected object
        if (null != Selection.activeGameObject)
        {
            sceneLoadRuntimeData.SelectedObjectPath = getSceneRelativePath(Selection.activeGameObject);
        }
        else
        {
            sceneLoadRuntimeData.SelectedObjectPath = "";
        }
    }

    public static void RestoreHierarchyState()
    {
        var sceneLoadRuntimeData = SOLoaderEditor.GetUniqueAsset<SceneLoadRuntimeDataSO>();

        // HACK: Expand scenes that were open, even though they may not have been expanded
        foreach (var scenePath in sceneLoadRuntimeData.OpenScenePaths)
        {
            var scene = SceneManager.GetSceneByPath(scenePath);
            if (!scene.IsValid()) continue;

            var sceneInstanceId = scene.handle;
            setExpandedMethod.Invoke(hierarchyWindow, new object[] { sceneInstanceId, true });
        }

        // Restore object expanded states
        foreach (var path in sceneLoadRuntimeData.ExpandedObjectPaths)
        {
            var gameObject = findObjectByPath(path);
            if (null == gameObject) continue;

            // Invoke the method
            setExpandedMethod.Invoke(hierarchyWindow, new object[] { gameObject.GetInstanceID(), true });
        }

        restoreSelection(sceneLoadRuntimeData.SelectedObjectPath);

        EditorApplication.RepaintHierarchyWindow();
    }

    private static string[] getExpandedPaths()
    {
        // Invoke the method on the existing instance
        var expandedIDs = (int[])getExpandedIDsMethod.Invoke(hierarchyWindow, null);
        if (null == expandedIDs || expandedIDs.Length == 0) return Array.Empty<string>();

        // Convert instance IDs to GameObject paths
        List<string> expandedPaths = new();
        foreach (int id in expandedIDs)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(id) as GameObject;
            if (null != gameObject)
            {
                expandedPaths.Add(getSceneRelativePath(gameObject));
            }
        }

        return expandedPaths.ToArray();
    }

    private static void restoreSelection(string selectedObjectPath)
    {
        if (string.IsNullOrEmpty(selectedObjectPath)) return;

        var selectedObject = findObjectByPath(selectedObjectPath);
        if (null == selectedObject) return;

        Selection.activeGameObject = selectedObject;
        EditorGUIUtility.PingObject(selectedObject);
    }

    private static string getSceneRelativePath(GameObject gameObject)
    {
        string path = gameObject.name;
        Transform parent = gameObject.transform.parent;

        while (null != parent)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return gameObject.scene.name + "/" + path;
    }

    private static GameObject findObjectByPath(string path)
    {
        string[] parts = path.Split('/');
        if (parts.Length < 2) return null;

        Scene scene = SceneManager.GetSceneByName(parts[0]);
        if (!scene.IsValid()) return null;

        GameObject gameObject = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == parts[1])
            {
                gameObject = root;
                break;
            }
        }

        if (null == gameObject) return null;

        for (int i = 2; i < parts.Length; i++)
        {
            Transform child = gameObject.transform.Find(parts[i]);
            if (child == null) return null;
            gameObject = child.gameObject;
        }

        return gameObject;
    }
}
