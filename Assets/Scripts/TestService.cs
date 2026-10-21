using BeardPhantom.Bootstrap;
using System;
using System.Threading;
using UnityEngine;

[Serializable]
public class TestService : IService
{
    [field: SerializeField]
    private int IntProperty { get; set; }

    [field: SerializeField]
    private float FloatProperty { get; set; }

    [field: SerializeField]
    private string StringProperty { get; set; }

    private static async Awaitable AsyncTask(CancellationToken cancellationToken = default)
    {
        var timer = 0f;
        const float Duration = 0.5f;
        while (timer < Duration)
        {
            await Awaitable.NextFrameAsync(cancellationToken);
            timer += Time.deltaTime;
        }
    }

    void IService.InitService(BootstrapContext context)
    {
        if (Application.isPlaying)
        {
            return;
        }

        context.Scheduler.Schedule(AsyncTask);
    }
}