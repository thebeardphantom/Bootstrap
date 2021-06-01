using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fabric.Core.Runtime
{
    /// <summary>
    /// A simple service container
    /// </summary>
    public abstract class ServiceModule : IDisposable, IInitable
    {
        #region Fields

        /// <summary>
        /// Cached service instances
        /// </summary>
        private readonly Dictionary<Type, object> _bindings = new Dictionary<Type, object>();

        #endregion

        #region Methods

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var binding in _bindings.Values)
            {
                if (binding is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _bindings.Clear();
        }

        public async UniTask InitAsync()
        {
            using (ListPool<UniTask>.Get(out var tasks, _bindings.Count))
            {
                foreach (var service in _bindings.Values)
                {
                    if (service is IInitable initable)
                    {
                        tasks.Add(initable.InitAsync());
                    }
                }

                await UniTask.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Function for subclasses to override to register services
        /// </summary>
        protected internal abstract void BindServices();

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
            _bindings.TryGetValue(serviceType, out var service);
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
            if (_bindings.ContainsKey(type))
            {
                _bindings[type] = provider;
            }
            else
            {
                _bindings.Add(type, provider);
            }

            if (provider is Object obj)
            {
                Object.DontDestroyOnLoad(obj);
            }

            return provider;
        }

        #endregion
    }
}