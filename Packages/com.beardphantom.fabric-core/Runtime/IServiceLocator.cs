using System;

namespace BeardPhantom.Fabric.Core
{
    public interface IServiceLocator : IDisposable
    {
        #region Methods

        bool TryLocateService<T>(out T service) where T : class;

        #endregion
    }

    public static class ServiceLocatorUtility
    {
        #region Methods

        public static T LocateService<T>(this IServiceLocator serviceLocator) where T : class
        {
            if (serviceLocator.TryLocateService<T>(out var service))
            {
                return service;
            }

            throw new Exception($"Service of type {typeof(T)} not found.");
        }

        #endregion
    }
}