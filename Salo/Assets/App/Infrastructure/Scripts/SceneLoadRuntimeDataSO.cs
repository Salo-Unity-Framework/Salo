using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneLoadRuntimeData", menuName = "Salo/Runtime Data/Scene Load Runtime Data")]
public class SceneLoadRuntimeDataSO : RuntimeDataSOBase
{
    [Tooltip("Subclasses of SceneResourceLoaderBase will add and remove themselves from this list")]
    public List<SceneResourceLoaderBase> SceneResourceLoaders = new();

    [Tooltip("Subclasses of SceneFaderBase will assign and remove themselves to this. To be used by SceneLoadManager")]
    public SceneFaderBase CurrentSceneFader;

#if UNITY_EDITOR

    [Header("Editor")]

    // NOTE: Values here persist from PlayModeStateChange of ExitingEditMode to EnteringPlayMode

    [Tooltip("Normally, FirstSceneLoader handles OnFirstSceneLoadRequested events. However, on"
        + " OpenSceneType.Others, EditorBootstrapper hijacks the event and takes over.")]
    public OpenSceneType CurrentOpenSceneType;

    [Tooltip("The paths to scenes that were open in the Editor before Play")]
    public string[] OpenScenePaths;

    [Tooltip("The paths to scene hierarchy objects that were expnaded on the Editor open scenes")]
    public string[] ExpandedObjectPaths;

    [Tooltip("The path to the selected object in the scene hierarchy")]
    public string SelectedObjectPath;

#endif
}
