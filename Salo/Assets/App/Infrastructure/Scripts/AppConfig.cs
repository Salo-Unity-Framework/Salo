using UnityEngine;

/// <summary>
/// This bootstrapped system hold references to the
/// various ConfigSOBase subclasses in the project
/// </summary>
public class AppConfig : StaticInstanceOf<AppConfig>
{
    [SerializeField] private SceneLoadConfigSO sceneLoadConfig;
    public SceneLoadConfigSO SceneLoadConfig => sceneLoadConfig;
}
