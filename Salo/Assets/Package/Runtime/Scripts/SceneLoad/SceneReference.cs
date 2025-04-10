#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine.AddressableAssets;

[Serializable]
public class SceneReference : AssetReference
{
    /// <summary>
    /// Constructs a new reference to a Scene Asset.
    /// </summary>
    /// <param name="guid">The object guid.</param>
    public SceneReference(string guid) : base(guid) { }

    public override bool ValidateAsset(string path)
    {
#if UNITY_EDITOR
        var type = AssetDatabase.GetMainAssetTypeAtPath(path);
        return type != null && typeof(SceneAsset).IsAssignableFrom(type);
#else
        return false;
#endif
    }

#if UNITY_EDITOR
    public new SceneAsset editorAsset
    {
        get
        {
            if (CachedAsset != null || string.IsNullOrEmpty(AssetGUID))
            {
                return CachedAsset as SceneAsset;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(AssetGUID);
            var main = AssetDatabase.LoadMainAssetAtPath(assetPath) as SceneAsset;
            if (main != null) CachedAsset = main;

            return main;
        }
    }
#endif
}
