using BeardPhantom.Bootstrap;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestService : MonoBehaviour, IBootstrapService
{
    private static async UniTask AsyncTask()
    {
        var timer = 0f;
        const float Duration = 0.5f;
        while (timer < Duration)
        {
            await UniTask.NextFrame();
            timer += Time.deltaTime;
        }
    }

    void IBootstrapService.InitService(BootstrapContext context)
    {
        if (Application.isPlaying)
        {
            return;
        }

        context.Scheduler.Schedule(new ScheduledTask(AsyncTask));
    }
}