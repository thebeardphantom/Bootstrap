using System;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace ASF.Core.Runtime
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
        public static void Populate<T>(ref T field) where T : class
        {
            field = Get<T>();
        }

        /// <summary>
        /// Populates two fields' values using the ServiceKernel
        /// </summary>
        public static void Populate<T1, T2>(ref T1 field1, ref T2 field2)
            where T1 : class
            where T2 : class
        {
            field1 = Get<T1>();
            field2 = Get<T2>();
        }

        /// <summary>
        /// Populates three fields' values using the ServiceKernel
        /// </summary>
        public static void Populate<T1, T2, T3>(ref T1 field1, ref T2 field2, ref T3 field3)
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
        public static void Populate<T1, T2, T3, T4>(ref T1 field1, ref T2 field2, ref T3 field3, ref T4 field4)
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

        public static async Task SetKernel(ServiceKernel kernel)
        {
            Assert.IsNotNull(kernel);
            _kernel?.Dispose();
            _kernel = kernel;
            KernelGuid = Guid.NewGuid();
            await kernel.BindAllModules();
        }

        public static T Get<T>() where T : class
        {
            return _kernel.Get<T>();
        }

        #endregion
    }
}