using BeardPhantom.Bootstrap.Environment;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Base <see cref="AppInstance" /> for runtime (non-edit mode) contexts. Selects a
    /// <see cref="BootstrapEnvironmentAsset" /> and drives it through the pre-bootstrap, service creation, and
    /// post-bootstrap sequence.
    /// </summary>
    public abstract class RuntimeAppInstance : AppInstance
    {
        private IPreBootstrapHandler _preHandler;

        private IPostBootstrapHandler _postHandler;

        /// <summary>
        /// The <see cref="BootstrapEnvironmentAsset" /> selected for the current session.
        /// </summary>
        public BootstrapEnvironmentAsset SessionEnvironment { get; private set; }

        /// <summary>
        /// The <see cref="ServiceList" /> of the current <see cref="SessionEnvironment" />.
        /// </summary>
        public override ServiceList ActiveServiceList => SessionEnvironment.IsNull() ? null : SessionEnvironment.ServiceListAsset;

        /// <summary>
        /// Gets the default pre/post bootstrap handlers to use when <see cref="SessionEnvironment" /> doesn't
        /// specify its own.
        /// </summary>
        public abstract void GetDefaultBootstrapHandlers(
            out IPreBootstrapHandler preBootstrapHandler,
            out IPostBootstrapHandler postBootstrapHandler);

        /// <summary>
        /// Determines the <see cref="BootstrapEnvironmentAsset" /> to use for this session.
        /// </summary>
        protected abstract bool TryLoadEnvironment(out BootstrapEnvironmentAsset environment);

        internal override async Awaitable BootstrapAsync()
        {
            await base.BootstrapAsync();

            var appInstance = App.Instance;
            var cancellationToken = Application.exitCancellationToken;

            if (TryLoadEnvironment(out var environmentAssetSource))
            {
                Logging.Info($"Selected environment {environmentAssetSource}.");
                if (environmentAssetSource.ServiceListAsset.IsNull())
                {
                    Logging.Error($"Environment {environmentAssetSource} has no assigned {nameof(ServiceList)}.");
                }
                else
                {
                    SessionEnvironment = environmentAssetSource.StartEnvironment();

                    appInstance.BootstrapState = AppBootstrapState.BootstrapHandlerDiscovery;
                    Logging.Info("Bootstrapping application.");
                    AssignBootstrapHandlers();

                    appInstance.BootstrapState = AppBootstrapState.PreBootstrap;
                    Logging.Trace("Beginning pre-bootstrapping.");
                    var context = new BootstrapContext(appInstance.TaskScheduler);
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
                }
            }
            else
            {
                Logging.Info("No environment selected.");
            }
            
            ComponentMessageRelay.Create();

            appInstance.BootstrapState = AppBootstrapState.Ready;
            Logging.Info("Bootstrapping complete.");
            await Awaitable.NextFrameAsync(cancellationToken);
        }

        private void AssignBootstrapHandlers()
        {
            GetDefaultBootstrapHandlers(
                out var defaultPreHandler,
                out var defaultPostHandler);
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
            var cancellationToken = linked.Token;
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