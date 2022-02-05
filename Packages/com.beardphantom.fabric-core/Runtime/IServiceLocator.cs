using System;

namespace BeardPhantom.Fabric.Core
{
    public interface IServiceLocator : IDisposable
    {
        #region Methods

        bool TryLocateService(Type serviceType, out object service);

        #endregion
    }

    public static class ServiceLocatorUtility
    {
        #region Methods

        public static bool TryLocateService<T>(this IServiceLocator serviceLocator, out T service)
            where T : class
        {
            if (serviceLocator.TryLocateService(typeof(T), out var rawObject))
            {
                service = (T)rawObject;
                return true;
            }

            service = default;
            return false;
        }

        public static T LocateService<T>(this IServiceLocator serviceLocator) where T : class
        {
            if (serviceLocator.TryLocateService<T>(out var service))
            {
                return service;
            }

            throw new Exception($"Service of type {typeof(T)} not found.");
        }

        public static object LocateService(this IServiceLocator serviceLocator, Type serviceType)
        {
            if (serviceLocator.TryLocateService(serviceType, out var service))
            {
                return service;
            }

            throw new Exception($"Service of type {serviceType} not found.");
        }

        #endregion
    }
}