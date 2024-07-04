using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    public class ServiceLocator : IServiceLocator
    {
        public delegate void OnServiceEvent(IBootstrapService service);

        public event OnServiceEvent ServiceDiscovered;

        public event OnServiceEvent ServiceEarlyInitialized;

        public event OnServiceEvent ServiceInitialized;

        public event OnServiceEvent ServiceLateInitialized;

        private readonly Dictionary<Type, IBootstrapService> _typeToServices = new();

        private readonly HashSet<IBootstrapService> _services = new();

        private GameObject _servicesInstance;

        private static async UniTask WaitThenFireEvent(
            OnServiceEvent onServiceEvent,
            UniTask serviceTask,
            IBootstrapService bootstrapService)
        {
            await serviceTask;
            onServiceEvent?.Invoke(bootstrapService);
        }

        public async UniTask CreateAsync(BootstrapContext context, GameObject servicesInstance, HideFlags hideFlags = HideFlags.DontSave)
        {
            _servicesInstance = servicesInstance;
            _servicesInstance.hideFlags = hideFlags;

            /*
             * Service Discovery
             */
            App.BootstrapState = AppBootstrapState.ServiceDiscovery;
            using (ListPool<IBootstrapService>.Get(out var serviceComponents))
            {
                _servicesInstance.GetComponentsInChildren(true, serviceComponents);
                foreach (var service in serviceComponents)
                {
                    var component = (Component)service;
                    component.hideFlags = hideFlags;
                    _services.Add(service);
                }
            }

            App.BootstrapState = AppBootstrapState.ServiceBinding;
            foreach (var service in _services)
            {
                var serviceType = service.GetType();
                _typeToServices.Add(serviceType, service);

                if (service is IMultiboundBootstrapService multiboundService)
                {
                    using (ListPool<Type>.Get(out var extraBindableTypes))
                    {
                        multiboundService.GetExtraBindableTypes(extraBindableTypes);
                        foreach (var extraType in extraBindableTypes)
                        {
                            _typeToServices.Add(extraType, service);
                        }
                    }
                }

                ServiceDiscovered?.Invoke(service);
            }

            /*
             * Service Early Init
             */
            App.BootstrapState = AppBootstrapState.ServiceEarlyInit;
            Log.Verbose("Early initializing services.");
            using (ListPool<UniTask>.Get(out var tasks))
            {
                foreach (var service in _services.OfType<IEarlyInitBootstrapService>())
                {
                    var earlyInitTask = service.EarlyInitServiceAsync(context);
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
                foreach (var service in _services)
                {
                    var initTask = service.InitServiceAsync(context);
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
                foreach (var service in _services.OfType<ILateInitBootstrapService>())
                {
                    var lateInitTask = service.LateInitServiceAsync(context);
                    tasks.Add(WaitThenFireEvent(ServiceLateInitialized, lateInitTask, service));
                }

                await UniTask.WhenAll(tasks);
            }

            App.BootstrapState = AppBootstrapState.ServiceActivation;
            Log.Verbose("Activating services.");
            _servicesInstance.SetActive(true);
        }

        public bool TryLocateService(Type serviceType, out IBootstrapService service)
        {
            Assert.IsTrue(App.CanLocateServices, "App.CanLocateServices");
            return _typeToServices.TryGetValue(serviceType, out service);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Log.Verbose("Disposing ServiceLocator.");
            foreach (var service in _services)
            {
                if (service is IDisposable disposable)
                {
                    Log.Verbose($"Disposing service {service.GetType()}.");
                    disposable.Dispose();
                }
            }

            Object.DestroyImmediate(_servicesInstance);
        }

        /// <inheritdoc />
        bool IServiceLocator.TryLocateService(Type serviceType, out object service)
        {
            var didFind = TryLocateService(serviceType, out var bootstrapService);
            service = bootstrapService;
            return didFind;
        }
    }
}