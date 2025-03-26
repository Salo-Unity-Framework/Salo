using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoadConfig", menuName = "Salo/Config/Scene Load Config SO")]
public class SceneLoadConfigSO : ConfigSOBase
{
    [Tooltip("The path to ZeroScene. Read from the Scene List, assuming it is the first scene")]
    public string ZeroScenePath => SceneUtility.GetScenePathByBuildIndex(0);

    [Tooltip("An Addressable reference to BootstrapScene")]
    [SerializeField] private SceneReference bootstrapScene;
    public SceneReference BootstrapScene => bootstrapScene;
}
