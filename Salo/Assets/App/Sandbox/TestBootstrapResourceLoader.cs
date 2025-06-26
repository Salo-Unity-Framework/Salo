using Cysharp.Threading.Tasks;
using Salo.Infrastructure;
using System.Diagnostics;
using UnityEngine;

public class TestBootstrapResourceLoader : BootstrapResourceLoaderBase
{
    [SerializeField] private string identifier; // Debug variable

    private float currentBatchStartTime;
    private const float BATCH_SECONDS = 0.040f; // 1s / 25 for 25fps minimum

    private Stopwatch stopwatch; // Debug variable

    public async override UniTask Load()
    {
        currentBatchStartTime = Time.realtimeSinceStartup;
        stopwatch = Stopwatch.StartNew();

        // Loading method A: For multiple small objects, spread the loading across multiple frames
        for (int i = 0; i < 10; i++)
        {
            stopwatch.Reset();
            stopwatch.Start();

            UnityEngine.Debug.Log($"{identifier} A{i}. Frame: {Time.frameCount}");
            while (stopwatch.ElapsedMilliseconds < 5)
            {
                // Simulate heavy synchronous task
            }

            // Continue in the next frame if the time threshold was crossed
            if (Time.realtimeSinceStartup - currentBatchStartTime >= BATCH_SECONDS)
            {
                await UniTask.Yield();
                currentBatchStartTime = Time.realtimeSinceStartup;
            }
        }

        // Loading method B: For heavier objects, run the whole thing on a separate thread
        UnityEngine.Debug.Log($"{identifier} B start frame: {Time.frameCount}");
        await System.Threading.Tasks.Task.Run(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();

                
                while (stopwatch.ElapsedMilliseconds < 100)
                {
                    // Simulate heavy synchronous task
                }
            }
        });

        UnityEngine.Debug.Log($"{identifier} B end frame: {Time.frameCount}");
    }
}
