using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeardPhantom.Fabric.Core
{
    public class ServiceLocator : IServiceLocator
    {
        #region Fields

        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        #endregion

        #region Methods

        public async UniTask CreateAsync(GameObject prefab)
        {
            prefab.SetActive(false);
            var services = Object.Instantiate(prefab);
            prefab.SetActive(true);

            Object.DontDestroyOnLoad(services);

            // Bind all services
            using (ListPool<IFabricService>.Get(out var foundServices))
            {
                services.GetComponentsInChildren(true, foundServices);
                foreach (var service in foundServices)
                {
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

            // Call OnCreateService on each service
            using (ListPool<UniTask>.Get(out var tasks))
            {
                foreach (var service in _services.Values)
                {
                    if (service is IFabricService fabricService)
                    {
                        tasks.Add(fabricService.InitServiceAsync());
                    }
                }

                // Wait for all
                await UniTask.WhenAll(tasks);
            }

            // Call OnAllServicesCreatedAsync on each service
            using (ListPool<UniTask>.Get(out var tasks))
            {
                foreach (var service in _services.Values)
                {
                    if (service is IFabricService fabricService)
                    {
                        tasks.Add(fabricService.PostInitAllServicesAsync());
                    }
                }

                // Wait for all
                await UniTask.WhenAll(tasks);
            }

            services.SetActive(true);
        }

        public bool TryLocateService(Type serviceType, out object service)
        {
            return _services.TryGetValue(serviceType, out service);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var service in _services.Values)
            {
                if (service is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        #endregion
    }
}