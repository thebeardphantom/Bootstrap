using System;

namespace BeardPhantom.Bootstrap
{
    public interface IServiceLocator : IDisposable
    {
        bool TryLocateService(Type serviceType, out object service);
    }
}