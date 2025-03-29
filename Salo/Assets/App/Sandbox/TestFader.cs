using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestFader : SceneFaderBase
{
    protected override void OnEnable()
    {
        SceneLoadEvents.OnSceneReady += handleSceneReady;
    }

    protected override void OnDisable()
    {
        SceneLoadEvents.OnSceneReady -= handleSceneReady;
    }

    private void handleSceneReady()
    {
        // Fade in here
        Debug.Log("Mock fader detected scene ready");
    }

    public async override UniTask FadeOut()
    {
        await UniTask.Delay(3000);
        Debug.Log("Mock fade out complete");
    }
}
