using BeardPhantom.Bootstrap;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class TestService : IServiceWithUpdateLoop
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
        var lastTimerInt = -1;
        const float Duration = 3f;
        while (timer < Duration)
        {
            await Awaitable.NextFrameAsync(cancellationToken);
            timer += Time.deltaTime;
            if ((int)timer != lastTimerInt)
            {
                lastTimerInt = (int)timer;
                Debug.Log($"TestService async task {lastTimerInt}");
            }
        }
    }

    void IService.InitService(BootstrapContext context)
    {
        context.Scheduler.Schedule(AsyncTask);
    }

    void IServiceWithUpdateLoop.Update(float deltaTime)
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            App.Reset();
        }
    }
}