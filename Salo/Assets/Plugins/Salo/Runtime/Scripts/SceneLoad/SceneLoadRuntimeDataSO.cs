using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Salo.Infrastructure
{
    [CreateAssetMenu(fileName = "SceneLoadRuntimeData", menuName = "Salo/Runtime Data/Scene Load Runtime Data")]
    public class SceneLoadRuntimeDataSO : RuntimeDataSOBase
    {
        // Subclasses of SceneResourceLoaderBase will add and remove themselves from this list
        private List<SceneResourceLoaderBase> sceneResourceLoaders;
        public List<SceneResourceLoaderBase> SceneResourceLoaders
        {
            get
            {
                if (null == sceneResourceLoaders) sceneResourceLoaders = new();
                return sceneResourceLoaders;
            }
        }

        [Tooltip("Currently loaded Addressable SceneReference")]
        public SceneReference LoadedSceneReference;

        [Tooltip("Currently loaded Scene struct")]
        public Scene LoadedScene;

        [Tooltip("Subclasses of SceneFaderBase will assign and remove themselves to this. To be used by SceneLoadManager")]
        public SceneFaderBase CurrentSceneFader;

#if UNITY_EDITOR

        [Header("Editor")]

        // NOTE: Values here persist from PlayModeStateChange of ExitingEditMode to EnteringPlayMode

        [Tooltip("The paths to scenes that were open in the Editor before Play")]
        public string[] OpenScenePaths;

        [Tooltip("The paths to scene hierarchy objects that were expnaded on the Editor open scenes")]
        public string[] ExpandedObjectPaths;

        [Tooltip("The path to the selected object in the scene hierarchy")]
        public string SelectedObjectPath;

        // Used as key in SessionState. Normally, FirstSceneLoader handles OnFirstSceneLoadRequested events.
        // However, on OpenSceneType.Others, EditorBootstrapper hijacks the event and takes over.
        public const string OPEN_SCENE_TYPE_KEY = "OPEN_SCENE_TYPE";
#endif
    }
}
