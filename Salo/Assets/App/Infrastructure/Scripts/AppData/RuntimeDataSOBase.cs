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
    // Pre-emptive implementation of IPersistable method since the
    // implementation will be the same for all Runtime data
    public virtual void ResetData()
    {
        ScriptableObjectHelper.ResetToTypeDefaults(this);

        // NOTE: Data is not yet written to disk. Save has to be called to do that
    }

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
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            ScriptableObjectHelper.ResetToTypeDefaults(this);
        }
    }

#endif
}
