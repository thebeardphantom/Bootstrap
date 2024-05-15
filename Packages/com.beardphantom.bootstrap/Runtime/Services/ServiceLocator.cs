using BeardPhantom.Bootstrap.Logging;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap.Services
{
    public class ServiceLocator : IServiceLocator
    {
        #region Types

        public delegate void OnServiceEvent(IBootstrapService service);

        #endregion

        #region Events

        public event OnServiceEvent ServiceDiscovered;

        public event OnServiceEvent ServiceEarlyInitialized;

        public event OnServiceEvent ServiceInitialized;

        public event OnServiceEvent ServiceLateInitialized;

        #endregion

        #region Fields

        private readonly Dictionary<Type, IBootstrapService> _services = new();

        #endregion

        #region Methods

        private static async UniTask WaitThenFireEvent(
            OnServiceEvent onServiceEvent,
            UniTask serviceTask,
            IBootstrapService bootstrapService)
        {
            await serviceTask;
            onServiceEvent?.Invoke(bootstrapService);
        }

        public async UniTask CreateAsync(GameObject prefab)
        {
            prefab.SetActive(false);
            var servicesInstance = Object.Instantiate(prefab);
            servicesInstance.name = prefab.name;
            prefab.SetActive(true);

            Object.DontDestroyOnLoad(servicesInstance);

            /*
             * Service Discovery
             */
            App.BootstrapState = AppBootstrapState.ServiceDiscovery;
            using (ListPool<IBootstrapService>.Get(out var foundServices))
            {
                servicesInstance.GetComponentsInChildren(true, foundServices);

                foreach (var service in foundServices)
                {
                    var serviceType = service.GetType();
                    _services.Add(serviceType, service);

                    if (service is IMultiboundBootstrapService multiboundService)
                    {
                        using (ListPool<Type>.Get(out var extraBindableTypes))
                        {
                            multiboundService.GetExtraBindableTypes(extraBindableTypes);
                            foreach (var extraType in extraBindableTypes)
                            {
                                _services.Add(extraType, service);
                            }
                        }
                    }

                    ServiceDiscovered?.Invoke(service);
                }
            }

            /*
             * Service Early Init
             */
            App.BootstrapState = AppBootstrapState.ServiceEarlyInit;
            Log.Verbose("Early initializing services.");
            using (ListPool<UniTask>.Get(out var tasks))
            {
                foreach (var service in _services.Values.OfType<IEarlyInitBootstrapService>())
                {
                    var earlyInitTask = service.EarlyInitServiceAsync();
                    tasks.Add(WaitThenFireEvent(ServiceEarlyInitialized, earlyInitTask, service));
                }

                await UniTask.WhenAll(tasks);
            }

            /*
             * Service Init
             */
            App.BootstrapState = AppBootstrapState.ServiceInit;
            Log.Verbose("Initializing services.");
            using (ListPool<UniTask>.Get(out var tasks))
            {
                foreach (var service in _services.Values)
                {
                    var initTask = service.InitServiceAsync();
                    tasks.Add(WaitThenFireEvent(ServiceInitialized, initTask, service));
                }

                await UniTask.WhenAll(tasks);
            }

            /*
             * Service Post Init
             */
            App.BootstrapState = AppBootstrapState.ServiceLateInit;
            Log.Verbose("Late initializing services.");
            using (ListPool<UniTask>.Get(out var tasks))
            {
                foreach (var service in _services.Values.OfType<ILateInitBootstrapService>())
                {
                    var lateInitTask = service.LateInitServiceAsync();
                    tasks.Add(WaitThenFireEvent(ServiceLateInitialized, lateInitTask, service));
                }

                await UniTask.WhenAll(tasks);
            }

            App.BootstrapState = AppBootstrapState.ServiceActivation;
            Log.Verbose("Activating services.");
            servicesInstance.SetActive(true);
        }

        public bool TryLocateService(Type serviceType, out IBootstrapService service)
        {
            Assert.IsTrue(App.CanLocateServices, "App.CanLocateServices");
            return _services.TryGetValue(serviceType, out service);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Log.Verbose("Disposing ServiceLocator.");
            foreach (var service in _services.Values)
            {
                if (service is IDisposable disposable)
                {
                    Log.Verbose($"Disposing service {service.GetType()}.");
                    disposable.Dispose();
                }
            }
        }

        /// <inheritdoc />
        bool IServiceLocator.TryLocateService(Type serviceType, out object service)
        {
            var didFind = TryLocateService(serviceType, out var bootstrapService);
            service = bootstrapService;
            return didFind;
        }

        #endregion
    }
}