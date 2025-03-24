using UnityEngine;

[CreateAssetMenu(fileName = "SceneLoadConfig", menuName = "Salo/Config/Scene Load Config SO")]
public class SceneLoadConfigSO : ConfigSOBase
{
    [Tooltip("A reference to BootstrapScene")]
    [SerializeField] private AssetReferenceScene bootstrapScene;
    public AssetReferenceScene BootstrapScene => bootstrapScene;
}
