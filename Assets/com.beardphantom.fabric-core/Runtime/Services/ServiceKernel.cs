using System;
using System.Collections.Generic;
using UniRx.Async;

namespace Fabric.Core.Runtime
{
    public sealed class ServiceKernel : IDisposable
    {
        #region Fields

        private readonly List<ServiceModule> _modules = new List<ServiceModule>();

        #endregion

        #region Methods

        public void RegisterModule(ServiceModule module)
        {
            _modules.Add(module);
        }

        /// <summary>
        /// Retrieves a service by generic StateType.
        /// </summary>
        internal T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        internal object Get(Type serviceType)
        {
            foreach (var module in _modules)
            {
                var instance = module.Get(serviceType);
                if (instance != null)
                {
                    return instance;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var module in _modules)
            {
                module.Dispose();
            }

            _modules.Clear();
        }

        internal async UniTask SetupAsync()
        {
            foreach (var module in _modules)
            {
                module.BindServices();
            }

            foreach (var module in _modules)
            {
                module.FireServicesBound();
            }

            var tasks = new List<UniTask>();
            foreach (var module in _modules)
            {
                tasks.Add(module.InitAsyncServicesAsync());
            }

            await UniTask.WhenAll(tasks);
        }

        #endregion
    }
}