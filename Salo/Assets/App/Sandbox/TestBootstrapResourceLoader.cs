using Cysharp.Threading.Tasks;
using Salo.Infrastructure;
using UnityEngine;

public class TestBootstrapResourceLoader : BootstrapResourceLoaderBase
{
    public async override UniTask Load()
    {
        await UniTask.Delay(3000);
        Debug.Log("Test bootstrap loading completed");
    }
}
