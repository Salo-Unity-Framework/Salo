using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Salo.Infrastructure.EditorExtensions
{
    /// <summary>
    /// This class is used by EditorBootstrapper to save and restore the scene objects' expnaded and selected states
    /// Object paths are saved instead of InstanceID since scenes will be reloaded as part of bootstrapping. Paths
    /// are saved with index numbers to differentiate them from siblings with the same name.
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
            var pathParts = new List<string>();
            var currentTransform = gameObject.transform;

            while (null != currentTransform)
            {
                // Assign index to same name objects so they can be identified correctly
                var index = getSiblingIndex(currentTransform, currentTransform.parent);

                // Eg: Cube, Cube[1], Cube[2]
                pathParts.Insert(0, index > 0 ? $"{currentTransform.name}[{index}]" : currentTransform.name);

                // Continue up till scene root object
                currentTransform = currentTransform.parent;
            }

            // Add Scene name at the top level
            return gameObject.scene.name + "/" + string.Join("/", pathParts); 
        }

        private static int getSiblingIndex(Transform target, Transform parent)
        {
            if (parent == null) // Root objects in scene
            {
                Scene scene = target.gameObject.scene;
                if (!scene.IsValid()) return 0;

                int index = 0;
                foreach (var rootObject in scene.GetRootGameObjects())
                {
                    if (rootObject == target.gameObject) return index; // Object found

                    // Increment index for same name root objects
                    if (rootObject.name == target.name) index++;
                }

                throw new ArgumentException($"Scene root object named {target.name} not found in Scene {target.gameObject.scene.name}");
            }
            else // Non-root objects
            {
                int index = 0;
                foreach (Transform sibling in parent)
                {
                    if (sibling == target) return index; // Object found

                    // Increment index for same name siblings
                    if (sibling.name == target.name) index++;
                }

                throw new ArgumentException($"Object named {target.name} not found in parent named {parent.name}");
            }
        }

        private static GameObject findObjectByPath(string path)
        {
            string[] pathParts = path.Split('/');
            if (pathParts.Length < 2) return null; // Should have scene + object name at least

            Scene scene = SceneManager.GetSceneByName(pathParts[0]); // Path starts with scene name
            if (!scene.IsValid()) return null;

            // Start with the root object and then traverse down its children as long as there are path parts
            var gameObject = findChildFromNameWithIndex(scene.GetRootGameObjects(), pathParts[1]);

            // Continue from the third path part (i = 2)
            for (int i = 2; i < pathParts.Length; i++)
            {
                gameObject = findChildFromNameFromIndex(gameObject.transform, pathParts[i]);
            }

            return gameObject;
        }

        // Cube[1] -> (Cube, 1)
        private static (string, int) parseNameWithIndex(string nameWithIndex)
        {
            string name;
            int index;

            int bracketIndex = nameWithIndex.LastIndexOf('[');
            if (bracketIndex != -1 && nameWithIndex.EndsWith("]"))
            {
                // Eg: Cube[1]
                name = nameWithIndex.Substring(0, bracketIndex);
                if (!int.TryParse(nameWithIndex.Substring(bracketIndex + 1, nameWithIndex.Length - bracketIndex - 2), out index))
                    throw new ArgumentException($"Invalid nameWIthIdnex: {nameWithIndex}");
            }
            else
            {
                // First siblings don't have index. Eg: Cube
                name = nameWithIndex;
                index = 0;
            }

            return (name, index);
        }

        private static GameObject findChildFromNameWithIndex(IEnumerable<GameObject> gameObjects, string nameWithIndex)
        {
            var (name, index) = parseNameWithIndex(nameWithIndex);

            int currentIndex = 0;
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.name != name) continue; // Ignore other objects
                if (currentIndex == index) return gameObject; // Object at the correct index
                currentIndex++; // Object with same name found but index not reached yet. Increment.
            }

            throw new ArgumentException($"Object not found for nameWithIndex: {nameWithIndex}");
        }

        private static GameObject findChildFromNameFromIndex(Transform parent, string nameWithIndex)
        {
            var (name, index) = parseNameWithIndex(nameWithIndex);

            int currentIndex = 0;
            foreach (Transform childTransform in parent)
            {
                if (childTransform.name != name) continue; // Ignore other objects
                if (currentIndex == index) return childTransform.gameObject; // Object at the correct index
                currentIndex++; // Object with same name found but index not reached yet. Increment.
            }

            throw new ArgumentException($"Object not found for nameWithIndex: {nameWithIndex}");
        }
    }
}
