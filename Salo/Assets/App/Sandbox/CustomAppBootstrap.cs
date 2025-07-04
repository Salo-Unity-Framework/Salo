using Cysharp.Threading.Tasks;
using Salo.Infrastructure;
using UnityEngine;

public class CustomAppBootstrap : AppBootstrap
{
    protected async override UniTaskVoid bootstrap()
    {
        Debug.Log("CustomAppBpptstrap");

        await UniTask.Delay(500); // and other custom stuff

        base.bootstrap().Forget();
    }
}
