using BeardPhantom.Bootstrap.Environment;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    public abstract class RuntimeAppInstance : AppInstance
    {
        private IPreBootstrapHandler _preHandler;

        private IPostBootstrapHandler _postHandler;

        public BootstrapEnvironmentAsset SessionEnvironment { get; private set; }

        public override ServiceList ActiveServiceList => SessionEnvironment.ServiceListAsset;

        protected abstract bool TryDetermineSessionEnvironment(out BootstrapEnvironmentAsset environment);

        protected abstract void GetDefaultBootstrapHandlers(
            out IPreBootstrapHandler preBootstrapHandler,
            out IPostBootstrapHandler postBootstrapHandler);

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
            GetDefaultBootstrapHandlers(
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
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                Application.exitCancellationToken,
                AppLifetimeCancellationToken);
            CancellationToken cancellationToken = linked.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Assert.IsTrue(BootstrapState >= AppBootstrapState.AsyncTaskFlush, "BootstrapState >= AppBootstrapState.AsyncTaskFlush");
                await TaskScheduler.FlushQueueAsync(cancellationToken);
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }
    }
}