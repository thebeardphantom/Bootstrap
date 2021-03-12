using System;

namespace BeardPhantom.Fabric.Core
{
    public partial class App : IDisposable
    {
        #region Fields

        private readonly ServiceLocator _serviceLocator = new ServiceLocator();

        #endregion

        #region Methods

        /// <inheritdoc />
        public void Dispose()
        {
            _serviceLocator?.Dispose();
        }

        #endregion
    }
}