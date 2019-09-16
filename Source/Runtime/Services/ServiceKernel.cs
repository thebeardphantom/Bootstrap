using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task BindAllModulesAsync()
        {
            var tasks = new List<Task>();

            foreach (var module in _modules)
            {
                tasks.Add(module.BindAllServicesAsync());
            }

            foreach (var module in _modules)
            {
                module.FireServicesBound();
            }

            await Task.WhenAll(tasks);
        }

        #endregion
    }
}