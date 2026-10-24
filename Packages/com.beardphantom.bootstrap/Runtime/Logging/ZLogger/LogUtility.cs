#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// Factory methods for creating <see cref="StaticSafeLogger" /> instances.
    /// </summary>
    public static class LogUtility
    {
        /// <summary>
        /// Gets a <see cref="StaticSafeLogger" /> for the category named after <typeparamref name="T" />.
        /// </summary>
        public static ILogger GetStaticLogger<T>()
        {
            return GetStaticLogger(typeof(T));
        }

        /// <summary>
        /// Gets a <see cref="StaticSafeLogger" /> for the category named after <paramref name="type" />.
        /// </summary>
        public static ILogger GetStaticLogger(Type type)
        {
            return new StaticSafeLogger(type.Name);
        }

        /// <summary>
        /// Gets a <see cref="StaticSafeLogger" /> for the given <paramref name="category" />.
        /// </summary>
        public static ILogger GetStaticLogger(string category)
        {
            return new StaticSafeLogger(category);
        }
    }
}
#endif