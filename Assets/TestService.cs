using BeardPhantom.Bootstrap;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestService : MonoBehaviour, IBootstrapService
{
    /// <param name="context"></param>
    /// <inheritdoc />
    public async UniTask InitServiceAsync(BootstrapContext context)
    {
        if (Application.isPlaying)
        {
            return;
        }

        var timer = 0f;
        const float Duration = 0.5f;
        while (timer < Duration)
        {
            await UniTask.NextFrame();
            timer += Time.deltaTime;
        }
    }
}