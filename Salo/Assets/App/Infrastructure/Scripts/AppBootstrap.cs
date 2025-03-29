using UnityEngine;

/// <summary>
/// This exists on the Infrastructure prefab. This script takes over the
/// app flow during bootstrapping. It starts from its Start method and
/// ends when it calls SceneLoadEvents.FirstSceneLoadRequested.
/// </summary>
public class AppBootstrap : MonoBehaviour
{
    // This is the entry point of the bootstrap flow
    private void Start()
    {
        // Check entitlement etc

        // Play splash media

        SceneLoadEvents.FirstSceneLoadRequested();
    }
}
