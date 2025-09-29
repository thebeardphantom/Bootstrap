using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal static class NullUtility
    {
        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj switch
            {
                null => true,
                Object uObj => uObj == null,
                _ => false,
            };
        }

        public static bool IsNotNull<T>(this T obj) where T : class
        {
            return !IsNull(obj);
        }

        public static T NullCoalesce<T>(this T obj, T valueIfNull) where T : class
        {
            return IsNull(obj) ? valueIfNull : obj;
        }
    }
}