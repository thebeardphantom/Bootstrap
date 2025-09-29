using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    public class ServiceLocator : IDisposable, IEnumerable<IService>
    {
        public delegate void OnServiceEvent(IService service);

        public event OnServiceEvent ServiceDiscovered;

        public event OnServiceEvent ServiceInitialized;

        private readonly Dictionary<Type, IService> _typeToServices = new();

        private readonly List<IService> _services = new();

        private ServiceListAsset _serviceListAsset;

        public bool CanLocateServices => App.Instance.BootstrapState > AppBootstrapState.ServiceInit;

        public void Create(BootstrapContext context, ServiceListAsset serviceListAsset)
        {
            _serviceListAsset = serviceListAsset;
            AppInstance appInstance = App.Instance;

            /*
             * Service Discovery
             */
            appInstance.BootstrapState = AppBootstrapState.ServiceDiscovery;
            foreach (IService service in serviceListAsset.Services)
            {
                _services.Add(service);
                ServiceDiscovered?.Invoke(service);
            }

            _services.Sort(ServiceInitComparer.Instance);

            /*
             * Service Binding
             */
            appInstance.BootstrapState = AppBootstrapState.ServiceBinding;
            foreach (IService service in _services)
            {
                if (service is IServiceWithCustomBindings serviceWithCustomBindings)
                {
                    using (ListPool<Type>.Get(out List<Type> bindingTypes))
                    {
                        serviceWithCustomBindings.GetCustomBindings(bindingTypes, out bool autoIncludeDeclaredType);
                        if (autoIncludeDeclaredType)
                        {
                            _typeToServices.Add(service.GetType(), service);
                        }

                        foreach (Type type in bindingTypes)
                        {
                            _typeToServices.Add(type, service);
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
            appInstance.BootstrapState = AppBootstrapState.ServiceInit;
            Logging.Trace("Initializing services.");
            foreach (IService service in _services)
            {
                Logging.Trace($"InitService on {service.GetType()}.");
                service.InitService(context);
            }
        }

        public void Dispose()
        {
            Logging.Trace("Disposing ServiceLocator.");
            foreach (IService service in _services)
            {
                if (service is IDisposable disposable)
                {
                    Logging.Trace($"Disposing service {service.GetType()}.");
                    disposable.Dispose();
                }
            }

            if (!Application.isEditor || BootstrapUtility.IsPersistent(_serviceListAsset))
            {
                return;
            }

            BootstrapUtility.DestroyReferenceImmediate(ref _serviceListAsset);
        }

        public bool TryLocateService<T>(out T service, bool throwIfCannotLocateServices = true) where T : class
        {
            if (TryLocateService(typeof(T), out IService untypedService, throwIfCannotLocateServices))
            {
                service = (T)untypedService;
                return true;
            }

            service = null;
            return false;
        }

        public bool TryLocateService(Type serviceType, out IService service, bool throwIfCannotLocateServices = true)
        {
            if (CanLocateServices)
            {
                return _typeToServices.TryGetValue(serviceType, out service);
            }

            if (throwIfCannotLocateServices)
            {
                throw new InvalidOperationException($"Attempted to locate service {serviceType} before services can be located.");
            }

            service = null;
            return false;
        }

        public T LocateService<T>() where T : class
        {
            return (T)LocateService(typeof(T));
        }

        public IService LocateService(Type serviceType)
        {
            return TryLocateService(serviceType, out IService service)
                ? service
                : throw new ServiceNotFoundException(serviceType);
        }

        public IEnumerator<IService> GetEnumerator()
        {
            return CanLocateServices
                ? _services.GetEnumerator()
                : throw new InvalidOperationException("Attempted to access services before services can be located.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}