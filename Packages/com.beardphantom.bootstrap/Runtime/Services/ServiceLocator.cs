using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    public class ServiceLocator : IDisposable
    {
        public delegate void OnServiceEvent(IBootstrapService service);

        public event OnServiceEvent ServiceDiscovered;

        public event OnServiceEvent ServiceEarlyInitialized;

        public event OnServiceEvent ServiceInitialized;

        public event OnServiceEvent ServiceLateInitialized;

        private readonly Dictionary<Type, IBootstrapService> _typeToServices = new();

        private readonly HashSet<IBootstrapService> _services = new();

        private GameObject _servicesInstance;

        public bool CanLocateServices => App.BootstrapState > AppBootstrapState.ServiceEarlyInit;

        public void Create(BootstrapContext context, GameObject servicesInstance, HideFlags hideFlags = HideFlags.DontSave)
        {
            _servicesInstance = servicesInstance;
            _servicesInstance.hideFlags = hideFlags;

            /*
             * Service Discovery
             */
            App.BootstrapState = AppBootstrapState.ServiceDiscovery;
            using (ListPool<IBootstrapService>.Get(out List<IBootstrapService> serviceComponents))
            {
                _servicesInstance.GetComponentsInChildren(true, serviceComponents);
                foreach (IBootstrapService service in serviceComponents)
                {
                    var component = (Component)service;
                    component.hideFlags = hideFlags;
                    _services.Add(service);
                    ServiceDiscovered?.Invoke(service);
                }
            }

            /*
             * Service Binding
             */
            App.BootstrapState = AppBootstrapState.ServiceBinding;
            foreach (IBootstrapService service in _services)
            {
                if (service is IMultiboundBootstrapService multiboundService)
                {
                    using (ListPool<Type>.Get(out List<Type> extraBindableTypes))
                    {
                        multiboundService.GetOverrideBindingTypes(extraBindableTypes);
                        foreach (Type extraType in extraBindableTypes)
                        {
                            _typeToServices.Add(extraType, service);
                        }
                    }
                }
                else
                {
                    Type serviceType = service.GetType();
                    _typeToServices.Add(serviceType, service);
                }
            }

            /*
             * Service Init
             */
            App.BootstrapState = AppBootstrapState.ServiceInit;
            Logging.Trace("Initializing services.");
            foreach (IBootstrapService service in _services)
            {
                Logging.Trace($"InitService on {service.GetType()}.");
                service.InitService(context);
            }

            App.BootstrapState = AppBootstrapState.ServiceActivation;
            Logging.Trace("Activating services.");
            _servicesInstance.SetActive(true);
        }

        public void Dispose()
        {
            Logging.Trace("Disposing ServiceLocator.");
            foreach (IBootstrapService service in _services)
            {
                if (service is IDisposable disposable)
                {
                    Logging.Trace($"Disposing service {service.GetType()}.");
                    disposable.Dispose();
                }
            }

            Object.DestroyImmediate(_servicesInstance);
        }

        public bool TryLocateService<T>(out T service) where T : class
        {
            if (TryLocateService(typeof(T), out IBootstrapService untypedService))
            {
                service = (T)untypedService;
                return true;
            }

            service = default;
            return false;
        }

        public bool TryLocateService(Type serviceType, out IBootstrapService service)
        {
            return _typeToServices.TryGetValue(serviceType, out service);
        }

        public T LocateService<T>() where T : class
        {
            if (TryLocateService<T>(out T service))
            {
                return service;
            }

            throw new Exception($"Service of type {typeof(T)} not found.");
        }

        public IBootstrapService LocateService(Type serviceType)
        {
            if (TryLocateService(serviceType, out IBootstrapService service))
            {
                return service;
            }

            throw new Exception($"Service of type {serviceType} not found.");
        }
    }
}