#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// Factory methods for creating <see cref="StaticSafeLogger"/> instances.
    /// </summary>
    public static class LogUtility
    {
        /// <summary>
        /// Gets a <see cref="StaticSafeLogger"/> for the category named after <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type whose name is used as the logger category.</typeparam>
        public static ILogger GetStaticLogger<T>()
        {
            return GetStaticLogger(typeof(T));
        }

        /// <summary>
        /// Gets a <see cref="StaticSafeLogger"/> for the category named after <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type whose name is used as the logger category.</param>
        public static ILogger GetStaticLogger(Type type)
        {
            return new StaticSafeLogger(type.Name);
        }

        /// <summary>
        /// Gets a <see cref="StaticSafeLogger"/> for the given <paramref name="category"/>.
        /// </summary>
        /// <param name="category">The logger category name.</param>
        public static ILogger GetStaticLogger(string category)
        {
            return new StaticSafeLogger(category);
        }
    }
}
#endif