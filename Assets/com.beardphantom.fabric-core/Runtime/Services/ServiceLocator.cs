using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Fabric.Core.Runtime
{
    public sealed class ServiceLocator : IDisposable, IInitable
    {
        #region Fields

        private readonly List<ServiceModule> _modules = new List<ServiceModule>();

        #endregion

        #region Methods

        public void RegisterModule(ServiceModule module)
        {
            _modules.Add(module);
            module.BindServices();
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

        public async UniTask InitAsync()
        {
            using (ListPool<UniTask>.Get(out var tasks, _modules.Count))
            {
                foreach (var module in _modules)
                {
                    tasks.Add(module.InitAsync());
                }

                await UniTask.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Retrieves a service by generic StateType.
        /// </summary>
        internal T Locate<T>() where T : class
        {
            return (T) Locate(typeof(T));
        }

        internal object Locate(Type serviceType)
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

        #endregion
    }
}