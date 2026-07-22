using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

#pragma warning disable CS0067 // Event is never used

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Discovers, binds, and initializes services from a <see cref="ServiceList" />, and allows locating
    /// them by type at runtime.
    /// </summary>
    public class ServiceLocator : IDisposable
    {
        /// <summary>
        /// Signature for events raised in response to a service lifecycle change.
        /// </summary>
        /// <param name="service">The service the event pertains to.</param>
        public delegate void OnServiceEvent(IService service);

        /// <summary>
        /// Raised when a service has been discovered, before binding and initialization.
        /// </summary>
        public event OnServiceEvent ServiceDiscovered;

        /// <summary>
        /// Raised when a service has finished initializing.
        /// </summary>
        public event OnServiceEvent ServiceInitialized;

        private readonly Dictionary<Type, IService> _typeToServices = new();

        private readonly List<IService> _services = new();

        private ServiceList _serviceList;

        /// <summary>
        /// Whether services are currently allowed to be located.
        /// </summary>
        public bool CanLocateServices => App.Instance.BootstrapState > AppBootstrapState.ServiceInit;

        /// <summary>
        /// Gets the number of <see cref="IService">IServices</see> contained in the <see cref="ServiceLocator" />.
        /// </summary>
        public int Count => _services.Count;

        /// <summary>
        /// Gets the <see cref="IService" /> at the given <paramref name="index" />.
        /// </summary>
        /// <returns>The <see cref="IService" /> at the specified index.</returns>
        /// <param name="index">The zero-based index of the <see cref="IService" /> to get.</param>
        /// <exception cref="InvalidOperationException"><see cref="CanLocateServices" /> is false.</exception>
        public IService this[int index] => CanLocateServices ? _services[index] : throw GetServiceAccessException();

        private static Exception GetServiceAccessException()
        {
            return new InvalidOperationException("Attempted to access services before services can be located.");
        }

        /// <summary>
        /// Discovers, binds, and initializes all services in <paramref name="serviceList" />.
        /// </summary>
        /// <param name="context">The bootstrap context passed to each service's initialization.</param>
        /// <param name="serviceList">The list of services to discover and initialize.</param>
        public void Create(BootstrapContext context, ServiceList serviceList)
        {
            _serviceList = serviceList;
            AppInstance appInstance = App.Instance;

            /*
             * Service Discovery
             */
            appInstance.BootstrapState = AppBootstrapState.ServiceDiscovery;
            foreach (IService service in serviceList.Services)
            {
                Logging.Debug($"Discovered service {service.GetType()}.");
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
                            Logging.Debug($"Binding service to {service.GetType()}.");
                            _typeToServices.Add(service.GetType(), service);
                        }

                        foreach (Type type in bindingTypes)
                        {
                            Logging.Debug($"Binding service to {type}.");
                            _typeToServices.Add(type, service);
                        }
                    }
                }
                else
                {
                    Type serviceType = service.GetType();
                    Logging.Debug($"Binding service to {serviceType}.");
                    _typeToServices.Add(serviceType, service);
                }
            }

            /*
             * Service Init
             */
            appInstance.BootstrapState = AppBootstrapState.ServiceInit;
            Logging.Debug("Initializing services.");
            foreach (IService service in _services)
            {
                Logging.Debug($"InitService on {service.GetType()}.");
                service.InitService(context);
            }
        }

        /// <summary>
        /// Disposes all services that implement <see cref="IDisposable" />, and destroys the cloned service list
        /// asset if applicable.
        /// </summary>
        public void Dispose()
        {
            Logging.Debug("Disposing ServiceLocator.");
            foreach (IService service in _services)
            {
                if (service is IDisposable disposable)
                {
                    Logging.Debug($"Disposing service {service.GetType()}.");
                    disposable.Dispose();
                }
            }

            if (!Application.isEditor || BootstrapUtility.IsPersistent(_serviceList))
            {
                return;
            }

            BootstrapUtility.DestroyReferenceImmediate(ref _serviceList);
        }

        /// <summary>
        /// Attempts to locate a service bound to type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of service to locate.</typeparam>
        /// <param name="service">The located service, or null if not found.</param>
        /// <param name="throwIfCannotLocateServices">
        /// If true, throws an <see cref="InvalidOperationException" />
        /// when called before <see cref="CanLocateServices" /> is true.
        /// </param>
        /// <returns>True if a service was located; otherwise false.</returns>
        public bool TryLocateService<T>(out T service, bool throwIfCannotLocateServices = false) where T : class
        {
            if (TryLocateService(typeof(T), out IService untypedService, throwIfCannotLocateServices))
            {
                service = (T)untypedService;
                return true;
            }

            service = null;
            return false;
        }

        /// <summary>
        /// Attempts to locate a service bound to <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceType">The type of service to locate.</param>
        /// <param name="service">The located service, or null if not found.</param>
        /// <param name="throwIfCannotLocateServices">
        /// If true, throws an <see cref="InvalidOperationException" />
        /// when called before <see cref="CanLocateServices" /> is true.
        /// </param>
        /// <returns>True if a service was located; otherwise false.</returns>
        public bool TryLocateService(Type serviceType, out IService service, bool throwIfCannotLocateServices = false)
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

        /// <summary>
        /// Locates a service bound to type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of service to locate.</typeparam>
        /// <exception cref="ServiceNotFoundException">Thrown if no service is bound to <typeparamref name="T" />.</exception>
        public T LocateService<T>() where T : class
        {
            return (T)LocateService(typeof(T));
        }

        /// <summary>
        /// Locates a service bound to <paramref name="serviceType" />.
        /// </summary>
        /// <param name="serviceType">The type of service to locate.</param>
        /// <exception cref="ServiceNotFoundException">Thrown if no service is bound to <paramref name="serviceType" />.</exception>
        public IService LocateService(Type serviceType)
        {
            return TryLocateService(serviceType, out IService service)
                ? service
                : throw new ServiceNotFoundException(serviceType);
        }
    }
}