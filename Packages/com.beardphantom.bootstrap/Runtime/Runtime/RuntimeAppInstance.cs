using BeardPhantom.Bootstrap.Environment;
using System.Threading;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public abstract class RuntimeAppInstance : AppInstance
    {
        private bool _isQuitting;

        public BootstrapEnvironmentAsset SessionEnvironment { get; private set; }

        public override bool IsQuitting => _isQuitting;

        protected abstract bool TryDetermineSessionEnvironment(out BootstrapEnvironmentAsset environment);

        internal override async Awaitable BootstrapAsync()
        {
            await base.BootstrapAsync();
            IsPlaying = true;

            Application.quitting += OnApplicationQuitting;
            UpdateLoopAsync().Forget();

            if (TryDetermineSessionEnvironment(out BootstrapEnvironmentAsset environment))
            {
                SessionEnvironment = environment;
                Logging.Info($"Selected environment {SessionEnvironment}.");
                environment.StartEnvironment();
            }
            else
            {
                Logging.Info("No environment selected.");
            }
        }

        private async Awaitable UpdateLoopAsync()
        {
            CancellationToken cancellationToken = Application.exitCancellationToken;
            while (Application.isPlaying)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await TaskScheduler.FlushQueueAsync(cancellationToken);
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }

        private void OnApplicationQuitting()
        {
            Application.quitting -= OnApplicationQuitting;
            _isQuitting = true;
        }
    }
}