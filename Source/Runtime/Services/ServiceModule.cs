using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public void FireServicesBound()
        {
            foreach (var service in Bindings.Values)
            {
                if (service is IPostKernelServicesBound postKernelServicesBound)
                {
                    postKernelServicesBound.OnServicesBound();
                }
            }
        }

        /// <summary>
        /// Retrieves a service by generic StateType.
        /// </summary>
        protected internal TS Get<TS>() where TS : class
        {
            Bindings.TryGetValue(typeof(TS), out var service);
            return (TS) service;
        }

        /// <summary>
        /// Creates a new instance of type TP and binds it to type TS
        /// </summary>
        protected TS Bind<TS, TP>() where TS : class where TP : TS
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

            return Bind<TS>((TP) tpInstance);
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

        internal async Task BindAllServicesAsync()
        {
            BindServices();
            var tasks = new List<Task>();
            foreach (var service in Bindings.Values)
            {
                if (service is IAsyncInitService asyncService)
                {
                    tasks.Add(asyncService.InitAsync());
                }
            }

            await Task.WhenAll(tasks);
        }

        #endregion
    }
}