using BeardPhantom.Bootstrap;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestService : MonoBehaviour, IBootstrapService
    {
        #region Methods

        /// <param name="context"></param>
        /// <inheritdoc />
        public async UniTask InitServiceAsync(BootstrapContext context)
        {
            if (Application.isPlaying)
            {
                return;
            }

            var timer = 0f;
            const float DURATION = 0.5f;
            while (timer < DURATION)
            {
                await UniTask.NextFrame();
                timer += Time.deltaTime;
            }
        }

        #endregion
    }
}