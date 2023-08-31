using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    public class ServiceLocator : IServiceLocator
    {
        #region Fields

        private readonly Dictionary<Type, object> _services = new();

        #endregion

        #region Methods

        public async UniTask CreateAsync(GameObject prefab)
        {
            prefab.SetActive(false);
            var servicesInstance = Object.Instantiate(prefab);
            servicesInstance.name = prefab.name;
            prefab.SetActive(true);

            Object.DontDestroyOnLoad(servicesInstance);

            // Bind all services
            using (ListPool<IBootstrapService>.Get(out var foundServices))
            {
                servicesInstance.GetComponentsInChildren(true, foundServices);
                foreach (var service in foundServices)
                {
                    Log.Verbose($"Found service {service.GetType()}.");
                    var serviceType = service.GetType();
                    _services.Add(serviceType, service);
                    using (ListPool<Type>.Get(out var extraBindableTypes))
                    {
                        service.GetExtraBindableTypes(extraBindableTypes);
                        foreach (var extraType in extraBindableTypes)
                        {
                            _services.Add(extraType, service);
                        }
                    }
                }
            }

            // Call InitServiceAsync on each service
            Log.Verbose("Initializing services.");
            using (ListPool<UniTask>.Get(out var tasks))
            {
                foreach (var service in _services.Values)
                {
                    if (service is IBootstrapService fabricService)
                    {
                        Log.Verbose($"Initializing service {service.GetType()}.");
                        tasks.Add(fabricService.InitServiceAsync());
                    }
                }

                // Wait for all
                await UniTask.WhenAll(tasks);
            }

            // Call PostInitAllServicesAsync on each service
            Log.Verbose("Post-initializing services.");
            using (ListPool<UniTask>.Get(out var tasks))
            {
                foreach (var service in _services.Values)
                {
                    if (service is IBootstrapService fabricService)
                    {
                        Log.Verbose($"Post-initializing service {service.GetType()}.");
                        tasks.Add(fabricService.PostInitAllServicesAsync());
                    }
                }

                // Wait for all
                await UniTask.WhenAll(tasks);
            }

            Log.Verbose("Activating services.");
            servicesInstance.SetActive(true);
        }

        public bool TryLocateService(Type serviceType, out object service)
        {
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

        #endregion
    }
}