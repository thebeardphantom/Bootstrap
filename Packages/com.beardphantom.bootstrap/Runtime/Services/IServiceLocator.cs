using System;

namespace BeardPhantom.Bootstrap
{
    public interface IServiceLocator : IDisposable
    {
        #region Methods

        bool TryLocateService(Type serviceType, out object service);

        #endregion
    }
}