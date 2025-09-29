using BeardPhantom.Bootstrap.Environment;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    public abstract class RuntimeAppInstance : AppInstance
    {
        private bool _isQuitting;

        private IPreBootstrapHandler _preHandler;

        private IPostBootstrapHandler _postHandler;

        public BootstrapEnvironmentAsset SessionEnvironment { get; private set; }

        public override ServiceListAsset ActiveServiceListAsset => SessionEnvironment.ServiceListAsset;

        public override bool IsQuitting => _isQuitting;

        protected abstract bool TryDetermineSessionEnvironment(out BootstrapEnvironmentAsset environment);

        internal override void NotifyQuitting()
        {
            _isQuitting = true;
        }

        internal override async Awaitable BootstrapAsync()
        {
            await base.BootstrapAsync();
            IsPlaying = true;
            ComponentMessageRelay.Create();

            if (!TryDetermineSessionEnvironment(out BootstrapEnvironmentAsset environmentAssetSource))
            {
                Logging.Info("No environment selected.");
                return;
            }

            Logging.Info($"Selected environment {environmentAssetSource}.");
            SessionEnvironment = environmentAssetSource.StartEnvironment();

            AppInstance appInstance = App.Instance;
            CancellationToken cancellationToken = Application.exitCancellationToken;

            var context = new BootstrapContext(appInstance.TaskScheduler);

            appInstance.BootstrapState = AppBootstrapState.BootstrapHandlerDiscovery;
            Logging.Info("Bootstrapping application.");
            AssignBootstrapHandlers();

            appInstance.BootstrapState = AppBootstrapState.PreBootstrap;
            Logging.Trace("Beginning pre-bootstrapping.");
            await _preHandler.OnPreBootstrapAsync(context);
            await Awaitable.NextFrameAsync(cancellationToken);

            appInstance.ServiceLocator.Create(context, SessionEnvironment.ServiceListAsset);
            await Awaitable.NextFrameAsync(cancellationToken);

            appInstance.BootstrapState = AppBootstrapState.AsyncTaskFlush;
            FlushTaskSchedulerLoopAsync().Forget();
            Logging.Trace($"Waiting for idle {nameof(TaskScheduler)}.");
            await AwaitableUtility.WaitUntil(() => appInstance.TaskScheduler.IsIdle, cancellationToken);

            appInstance.BootstrapState = AppBootstrapState.PostBootstrap;
            Logging.Trace("Beginning post-bootstrapping.");
            await _postHandler.OnPostBootstrapAsync(context);
            await Awaitable.NextFrameAsync(cancellationToken);

            appInstance.BootstrapState = AppBootstrapState.Ready;
            Logging.Info("Bootstrapping complete.");
            await Awaitable.NextFrameAsync(cancellationToken);
        }

        private void AssignBootstrapHandlers()
        {
            BootstrapUtility.GetDefaultBootstrapHandlers(
                out IPreBootstrapHandler defaultPreHandler,
                out IPostBootstrapHandler defaultPostHandler);
            _preHandler = SessionEnvironment.PreBootstrapHandler.NullCoalesce(defaultPreHandler);
            _postHandler = SessionEnvironment.PostBootstrapHandler.NullCoalesce(defaultPostHandler);

            Logging.Trace($"Selected IPreBootstrapHandler {_preHandler}.");
            Logging.Trace($"Selected IPostBootstrapHandler {_postHandler}.");
        }

        private async Awaitable FlushTaskSchedulerLoopAsync()
        {
            Logging.Debug($"Starting {nameof(FlushTaskSchedulerLoopAsync)}()");
            CancellationToken cancellationToken = Application.exitCancellationToken;
            while (Application.isPlaying)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Assert.IsTrue(BootstrapState >= AppBootstrapState.AsyncTaskFlush, "BootstrapState >= AppBootstrapState.AsyncTaskFlush");
                await TaskScheduler.FlushQueueAsync(cancellationToken);
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }
    }
}