using System;

namespace Fabric.Core.Runtime
{
    public sealed partial class App : IServiceLocator
    {
        #region Fields

        public readonly ServiceLocator ServiceLocator = new ServiceLocator();

        #endregion

        #region Methods

        /// <inheritdoc />
        public void Dispose()
        {
            ServiceLocator?.Dispose();
        }

        /// <inheritdoc />
        public bool TryLocateService<T>(out T service) where T : class
        {
            return ServiceLocator.TryLocateService(out service);
        }

        #endregion
    }
}