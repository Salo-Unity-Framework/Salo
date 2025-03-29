using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestSceneResourceLoader : SceneResourceLoaderBase
{
    public async override UniTask Load()
    {
        await UniTask.Delay(1000);
        Debug.Log("Test loading completed");
    }

    public async override UniTask Unload()
    {
        await UniTask.Delay(1000);
        Debug.Log("Test loading completed");
    }
}
