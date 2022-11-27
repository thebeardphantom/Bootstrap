using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
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

            void OnApplicationQuitting()
            {
                IsQuitting = true;
                Application.quitting -= OnApplicationQuitting;
            }

            Application.quitting += OnApplicationQuitting;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public void Dispose()
        {
            ServiceLocator?.Dispose();
        }

        /// <inheritdoc />
        public bool TryLocateService(Type serviceType, out object service)
        {
            return ServiceLocator.TryLocateService(serviceType, out service);
        }

        #endregion
    }
}