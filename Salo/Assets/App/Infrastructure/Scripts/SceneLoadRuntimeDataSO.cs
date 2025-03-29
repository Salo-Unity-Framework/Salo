using UnityEngine;

[CreateAssetMenu(fileName = "SceneLoadRuntimeData", menuName = "Salo/Runtime Data/Scene Load Runtime Data")]
public class SceneLoadRuntimeDataSO : RuntimeDataSOBase
{

#if UNITY_EDITOR
    /// <summary>
    /// Normally, FirstSceneLoader handles OnFirstSceneLoadRequested events. However, on
    /// OpenSceneType.Others, EditorBootstrapper hijacks the event and takes over.
    /// </summary>
    public OpenSceneType CurrentOpenSceneType;
#endif

}
