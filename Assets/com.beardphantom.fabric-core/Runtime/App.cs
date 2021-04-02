using System;

namespace BeardPhantom.Fabric.Core
{
    public sealed partial class App : IDisposable
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

        #endregion
    }
}