using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal static class NullUtility
    {
        #region Methods

        public static bool IsNotNull<T>(this T obj) where T : class
        {
            return obj switch
            {
                Object unityObj => unityObj != null,
                null => false,
                var _ => true
            };
        }

        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj switch
            {
                Object unityObj => unityObj == null,
                null => true,
                var _ => false
            };
        }

        #endregion
    }
}