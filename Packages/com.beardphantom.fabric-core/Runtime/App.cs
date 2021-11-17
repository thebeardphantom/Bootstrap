using System;

namespace BeardPhantom.Fabric.Core
{
    public sealed partial class App : IServiceLocator
    {
        #region Fields

        public readonly ServiceLocator ServiceLocator = new ServiceLocator();

        private readonly Guid _sessionGuid;

        #endregion

        #region Constructors

        private App()
        {
            _sessionGuid = Guid.NewGuid();
        }

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