using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Base class for Runtime Data SOs. Subclass SOs should contain data that
/// do not persist across app sessions and will be changed during runtime.
/// On exiting Editor Play, serialized fields will be reset to default.
/// </summary>
public abstract class RuntimeDataSOBase : ScriptableObject
{
#if UNITY_EDITOR

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += handlePlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= handlePlayModeStateChanged;
    }

    private void handlePlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode) resetToDefault();
    }

    private void resetToDefault()
    {
        // Create a new instance - this will have default values
        var tempInstance = CreateInstance(GetType());
        var defaultJsonString = JsonUtility.ToJson(tempInstance);

        // Assign values from the instance with default values
        JsonUtility.FromJsonOverwrite(defaultJsonString, this);
        DestroyImmediate(tempInstance);
    }

#endif
}
