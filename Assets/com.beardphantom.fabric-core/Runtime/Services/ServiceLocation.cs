using System;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine.Assertions;

namespace Fabric.Core.Runtime
{
    public static class ServiceLocation
    {
        #region Fields

        /// <summary>
        /// Accessor for current service locator
        /// </summary>
        private static ServiceKernel _kernel;

        #endregion

        #region Properties

        /// <summary>
        /// Guid used for tracking changes to the ServiceKernel
        /// </summary>
        public static Guid KernelGuid { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Populates a field's value using the ServiceKernel
        /// </summary>
        public static void Populate<T>(out T field) where T : class
        {
            field = Get<T>();
        }

        /// <summary>
        /// Populates two fields' values using the ServiceKernel
        /// </summary>
        public static void Populate<T1, T2>(out T1 field1, out T2 field2)
            where T1 : class
            where T2 : class
        {
            field1 = Get<T1>();
            field2 = Get<T2>();
        }

        /// <summary>
        /// Populates three fields' values using the ServiceKernel
        /// </summary>
        public static void Populate<T1, T2, T3>(out T1 field1, out T2 field2, out T3 field3)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            field1 = Get<T1>();
            field2 = Get<T2>();
            field3 = Get<T3>();
        }

        /// <summary>
        /// Populates four fields' values using the ServiceKernel
        /// </summary>
        public static void Populate<T1, T2, T3, T4>(out T1 field1, out T2 field2, out T3 field3, out T4 field4)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            field1 = Get<T1>();
            field2 = Get<T2>();
            field3 = Get<T3>();
            field4 = Get<T4>();
        }

        public static async UniTask SetKernelAsync(ServiceKernel kernel)
        {
            Assert.IsNotNull(kernel);
            _kernel?.Dispose();
            _kernel = kernel;
            KernelGuid = Guid.NewGuid();
            await kernel.SetupAsync();
        }

        public static T Get<T>() where T : class
        {
            return (T)Get(typeof(T));
        }

        public static object Get(Type serviceType)
        {
            var service = _kernel.Get(serviceType);
            Assert.IsNotNull(service, $"No service of type {serviceType} bound.");
            return service;
        }

        #endregion
    }
}