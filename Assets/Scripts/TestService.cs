using BeardPhantom.Bootstrap;
using System;
using UnityEngine;

[Serializable]
public class TestService : IBootstrapService
{
    [field: SerializeField]
    private int IntProperty { get; set; }

    [field: SerializeField]
    private float FloatProperty { get; set; }

    [field: SerializeField]
    private string StringProperty { get; set; }

    private static async Awaitable AsyncTask()
    {
        var timer = 0f;
        const float Duration = 0.5f;
        while (timer < Duration)
        {
            await Awaitable.NextFrameAsync();
            timer += Time.deltaTime;
        }
    }

    void IBootstrapService.InitService(BootstrapContext context)
    {
        if (Application.isPlaying)
        {
            return;
        }

        context.Scheduler.Schedule(new AsyncScheduledTask(AsyncTask));
    }
}