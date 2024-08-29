using BeardPhantom.Bootstrap;
using UnityEngine;

public class TestService : MonoBehaviour, IBootstrapService
{
    /// <inheritdoc />
    async Awaitable IBootstrapService.InitServiceAsync(BootstrapContext context)
    {
        if (Application.isPlaying)
        {
            return;
        }

        var timer = 0f;
        const float Duration = 0.5f;
        while (timer < Duration)
        {
            await Awaitable.NextFrameAsync();
            timer += Time.deltaTime;
        }
    }
}