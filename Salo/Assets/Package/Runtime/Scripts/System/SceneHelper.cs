using UnityEngine.SceneManagement;

namespace Salo.Infrastructure
{
    public static class SceneHelper
    {
        // NOTE: This assumes a particluar order in the Scene List:
        // 0 - ZeroScene
        // 1 - BootstrapScene
        public static string ZeroScenePath => SceneUtility.GetScenePathByBuildIndex(0);
        public static string BootstrapScenePath => SceneUtility.GetScenePathByBuildIndex(1);
    }
}
