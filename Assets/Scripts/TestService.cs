using BeardPhantom.Bootstrap;
using UnityEngine;

public class TestService : MonoBehaviour, IBootstrapService
{
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

    void IBootstrapService.InitService(AsyncTaskScheduler scheduler)
    {
        if (Application.isPlaying)
        {
            return;
        }

        scheduler.Schedule(new ScheduledTask(AsyncTask));
    }
}