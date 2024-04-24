using System;

namespace BeardPhantom.Bootstrap.Services
{
    public interface IServiceLocator : IDisposable
    {
        #region Methods

        bool TryLocateService(Type serviceType, out object service);

        #endregion
    }
}