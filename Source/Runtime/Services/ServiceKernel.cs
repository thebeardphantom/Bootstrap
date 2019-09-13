using RSG;
using System;
using System.Collections.Generic;

namespace ASF.Core.Runtime
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
        public TS Get<TS>() where TS : class
        {
            foreach (var module in _modules)
            {
                var instance = module.Get<TS>();
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

        public IPromise BindAllModules()
        {
            var promises = new List<IPromise>();

            foreach (var module in _modules)
            {
                promises.Add(module.BindAllServices());
            }

            foreach (var module in _modules)
            {
                module.FireServicesBound();
            }

            return Promise.All(promises).WithName($"Bind {nameof(ServiceKernel)}");
        }

        #endregion
    }
}