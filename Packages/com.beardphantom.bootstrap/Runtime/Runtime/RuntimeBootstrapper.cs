using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    public sealed class RuntimeBootstrapper : MonoBehaviour
    {
        private IPreBootstrapHandler _preHandler;

        private IPostBootstrapHandler _postHandler;

        private bool _isOverrideInstance;

        [field: SerializeField]
        internal PrefabProvider PrefabProvider { get; set; }

        private void AssignBootstrapHandlers()
        {
            bool foundPreHandler = TryGetComponent(out _preHandler);
            bool foundPostHandler = TryGetComponent(out _postHandler);

            BootstrapUtility.GetDefaultBootstrapHandlers(
                out IPreBootstrapHandler defautlPreHandler,
                out IPostBootstrapHandler defaultPostHandler);
            _preHandler = foundPreHandler ? _preHandler : defautlPreHandler;
            _postHandler = foundPostHandler ? _postHandler : defaultPostHandler;

            Logging.Trace($"Selected IPreBootstrapHandler {_preHandler}.", this);
            Logging.Trace($"Selected IPostBootstrapHandler {_postHandler}.", this);
        }

        private void Start()
        {
            BootstrapApplicationAsync().Forget();
        }

        private async Awaitable BootstrapApplicationAsync()
        {
            AppInstance appInstance = App.Instance;
            if (appInstance.BootstrapState == AppBootstrapState.None)
            {
                CancellationToken cancellationToken = destroyCancellationToken;

                var context = new BootstrapContext(this, appInstance.TaskScheduler);
                Assert.IsNotNull(PrefabProvider, "ServicesPrefabLoader != null");

                appInstance.BootstrapState = AppBootstrapState.BootstrapHandlerDiscovery;
                Logging.Info("Bootstrapping application.", this);
                AssignBootstrapHandlers();

                appInstance.BootstrapState = AppBootstrapState.PreBootstrap;
                Logging.Trace("Beginning pre-bootstrapping.", this);
                await _preHandler.OnPreBootstrapAsync(context);
                await Awaitable.NextFrameAsync(cancellationToken);

                appInstance.BootstrapState = AppBootstrapState.ServicePrefabLoad;
                Logging.Trace($"Loading services prefab via loader {PrefabProvider}.", this);
                GameObject servicesPrefab = await PrefabProvider.LoadPrefabAsync();

                appInstance.BootstrapState = AppBootstrapState.ServiceCreation;
                Logging.Trace("Creating services.", this);
                servicesPrefab.SetActive(false);
                GameObject servicesInstance = Instantiate(servicesPrefab);
                DontDestroyOnLoad(servicesInstance);
                servicesInstance.name = servicesPrefab.name;
                servicesPrefab.SetActive(true);
                BootstrapUtility.ClearDirtyFlag(servicesPrefab);

                appInstance.ServiceLocator.Create(context, servicesInstance);
                await Awaitable.NextFrameAsync(cancellationToken);

                appInstance.BootstrapState = AppBootstrapState.AsyncTaskFlush;
                Logging.Trace($"Waiting for idle {nameof(TaskScheduler)}.", this);
                await AwaitableUtility.WaitUntil(() => appInstance.TaskScheduler.IsIdle, cancellationToken);

                appInstance.ServiceLocator.ActivateServicesObject();

                appInstance.BootstrapState = AppBootstrapState.PostBootstrap;
                Logging.Trace("Beginning post-bootstrapping.", this);
                await _postHandler.OnPostBootstrapAsync(context, this);
                await Awaitable.NextFrameAsync(cancellationToken);

                appInstance.BootstrapState = AppBootstrapState.Ready;
                Logging.Info("Bootstrapping complete.", this);
                await Awaitable.NextFrameAsync(cancellationToken);
            }

            Destroy(gameObject);
        }
    }
}