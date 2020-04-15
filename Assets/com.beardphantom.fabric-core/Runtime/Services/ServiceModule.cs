using System;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ASF.Core.Runtime
{
    /// <summary>
    /// A simple service container
    /// </summary>
    public abstract class ServiceModule : IDisposable
    {
        #region Fields

        /// <summary>
        /// Cached service instances
        /// </summary>
        internal readonly Dictionary<Type, object> Bindings = new Dictionary<Type, object>();

        private readonly HashSet<object> _visitorSet = new HashSet<object>();

        #endregion

        #region Methods

        /// <summary>
        /// Function for subclasses to override to register services
        /// </summary>
        protected internal abstract void BindServices();

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var binding in Bindings.Values)
            {
                if (binding is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            Bindings.Clear();
        }

        /// <summary>
        /// Retrieves a service by generic StateType.
        /// </summary>
        protected internal T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        /// <summary>
        /// Retrieves a service by generic StateType.
        /// </summary>
        protected internal object Get(Type serviceType)
        {
            Bindings.TryGetValue(serviceType, out var service);
            return service;
        }

        /// <summary>
        /// Creates a new instance of type TP and binds it to type TS
        /// </summary>
        protected TP Bind<TS, TP>() where TS : class where TP : TS
        {
            object tpInstance;

            if (typeof(ScriptableObject).IsAssignableFrom(typeof(TP)))
            {
                tpInstance = ScriptableObject.CreateInstance(typeof(TP));
            }
            else if (typeof(Component).IsAssignableFrom(typeof(TP)))
            {
                tpInstance = new GameObject(typeof(TP).Name).AddComponent(typeof(TP));
            }
            else
            {
                tpInstance = Activator.CreateInstance<TP>();
            }

            return (TP) Bind<TS>((TP) tpInstance);
        }

        /// <summary>
        /// Binds an existing provider instance to a service.
        /// </summary>
        protected TS Bind<TS>(TS provider) where TS : class
        {
            return (TS) Bind(typeof(TS), provider);
        }

        /// <summary>
        /// Registers a provider
        /// </summary>
        protected object Bind(Type type, object provider)
        {
            if (Bindings.ContainsKey(type))
            {
                Bindings[type] = provider;
            }
            else
            {
                Bindings.Add(type, provider);
            }

            if (provider is Object obj)
            {
                Object.DontDestroyOnLoad(obj);
            }

            return provider;
        }

        internal void FireServicesBound()
        {
            foreach (var service in Bindings.Values)
            {
                if (service is IPostKernelServicesBound postKernelServicesBound
                    && !_visitorSet.Contains(service))
                {
                    _visitorSet.Add(service);
                    postKernelServicesBound.OnServicesBound();
                }
            }

            _visitorSet.Clear();
        }

        internal async UniTask InitAsyncServicesAsync()
        {
            var tasks = new List<UniTask>();
            foreach (var service in Bindings.Values)
            {
                if (service is IAsyncInitService asyncService
                    && !_visitorSet.Contains(service))
                {
                    _visitorSet.Add(service);
                    tasks.Add(asyncService.InitAsync());
                }
            }

            _visitorSet.Clear();
            await UniTask.WhenAll(tasks);
        }

        #endregion
    }
}