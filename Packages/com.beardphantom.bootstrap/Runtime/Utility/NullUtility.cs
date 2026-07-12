using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal static class NullUtility
    {
        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("null => true; notnull => false")]
        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj switch
            {
                null => true,
                Object uObj => uObj == null,
                _ => false,
            };
        }

        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("null => true; notnull => false")]
        public static bool IsNull(this object obj)
        {
            return obj switch
            {
                null => true,
                Object uObj => uObj == null,
                _ => false,
            };
        }

        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("null => false; notnull => true")]
        public static bool IsNotNull<T>(this T obj) where T : class
        {
            return !obj.IsNull();
        }

        [System.Diagnostics.Contracts.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NullCoalesce<T>(this T obj, T valueIfNull) where T : class
        {
            return obj.IsNull() ? valueIfNull : obj;
        }
    }
}