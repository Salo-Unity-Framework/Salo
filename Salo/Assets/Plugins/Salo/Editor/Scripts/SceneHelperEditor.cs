using UnityEngine.SceneManagement;

namespace Salo.Infrastructure.EditorExtensions
{
    public static class SceneHelperEditor
    {
        // NOTE: This assumes the Scene List to contain ZeroScene at index-0.
        public static string ZeroScenePath => SceneUtility.GetScenePathByBuildIndex(0);
    }
}
