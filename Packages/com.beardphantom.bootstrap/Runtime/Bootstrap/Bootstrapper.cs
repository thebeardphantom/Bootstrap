﻿using System.Diagnostics.CodeAnalysis;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    public sealed partial class Bootstrapper : MonoBehaviour
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

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "Unity.IncorrectMethodSignature")]
        private async Awaitable Start()
        {
            Assert.IsTrue(gameObject.scene.buildIndex == 0, "gameObject.scene.buildIndex == 0");

#if UNITY_EDITOR
            if (!_isOverrideInstance)
            {
                TryReplaceWithOverrideInstance();
                if (this == null)
                {
                    // this instance was destroyed
                    return;
                }
            }
#endif

            CancellationToken cancellationToken = destroyCancellationToken;

            var context = new BootstrapContext(this, App.AsyncTaskScheduler);
            Assert.IsNotNull(PrefabProvider, "ServicesPrefabLoader != null");

            App.BootstrapState = AppBootstrapState.BootstrapHandlerDiscovery;
            Logging.Info("Bootstrapping application.", this);
            AssignBootstrapHandlers();

            App.BootstrapState = AppBootstrapState.PreBootstrap;
            Logging.Trace("Beginning pre-bootstrapping.", this);
            await _preHandler.OnPreBootstrapAsync(context);
            await Awaitable.NextFrameAsync(cancellationToken);

            App.BootstrapState = AppBootstrapState.ServicePrefabLoad;
            Logging.Trace($"Loading services prefab via loader {PrefabProvider}.", this);
            GameObject servicesPrefab = await PrefabProvider.LoadPrefabAsync();

            App.BootstrapState = AppBootstrapState.ServiceCreation;
            Logging.Trace("Creating services.", this);
            servicesPrefab.SetActive(false);
            GameObject servicesInstance = Instantiate(servicesPrefab);
            DontDestroyOnLoad(servicesInstance);
            servicesInstance.name = servicesPrefab.name;
            servicesPrefab.SetActive(true);
            BootstrapUtility.ClearDirtyFlag(servicesPrefab);
            App.ServiceLocator.Create(context, servicesInstance, HideFlags.None);
            await Awaitable.NextFrameAsync(cancellationToken);

            Logging.Trace($"Waiting for idle {nameof(AsyncTaskScheduler)}.", this);
            await AwaitableUtility.WaitUntil(() => App.AsyncTaskScheduler.IsIdle, cancellationToken);

            App.BootstrapState = AppBootstrapState.PostBoostrap;
            Logging.Trace("Beginning post-bootstrapping.", this);
            await _postHandler.OnPostBootstrapAsync(context, this);
            await Awaitable.NextFrameAsync(cancellationToken);

            App.BootstrapState = AppBootstrapState.Ready;
            Logging.Info("Bootstrapping complete.", this);
        }

        partial void TryReplaceWithOverrideInstance();
    }
}