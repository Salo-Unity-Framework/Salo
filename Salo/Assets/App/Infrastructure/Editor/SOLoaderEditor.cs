using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Easy access to SO assets like config assets
/// </summary>
public static class SOLoaderEditor
{
    private static Dictionary<Type, ScriptableObject> uniqueAssets = new();

    /// <summary>
    /// Get the asset assuming there is ever only one asset of its type in
    /// the project. If there are multiple instances, return the first
    /// </summary>
    public static T GetUniqueAsset<T>()
        where T : ScriptableObject
    {
        if (!uniqueAssets.ContainsKey(typeof(T)))
        {
            // Asset not assigned to the dictionary yet. Find and assign
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            Assert.IsTrue(guids.Length > 0);

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            uniqueAssets[typeof(T)] = AssetDatabase.LoadAssetAtPath<SceneLoadConfigSO>(path);
        }

        return (T)uniqueAssets[typeof(T)];
    }
}
