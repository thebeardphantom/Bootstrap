#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// Extension methods for <see cref="ILogService" />.
    /// </summary>
    public static class LogServiceExtensions
    {
        /// <summary>
        /// Attempts to get a logger for the category named after <typeparamref name="T" />.
        /// </summary>
        public static bool TryGetLogger<T>(this ILogService logService, out ILogger logger)
        {
            return logService.TryGetLogger(typeof(T).Name, out logger);
        }

        /// <summary>
        /// Gets a logger for the category named after <typeparamref name="T" />.
        /// </summary>
        public static ILogger GetLogger<T>(this ILogService logService)
        {
            logService.TryGetLogger(typeof(T).Name, out var logger);
            return logger;
        }

        /// <summary>
        /// Gets a logger for the category named after <paramref name="type" />.
        /// </summary>
        public static ILogger GetLogger(this ILogService logService, Type type)
        {
            logService.TryGetLogger(type.Name, out var logger);
            return logger;
        }

        /// <summary>
        /// Gets a logger for the given <paramref name="category" />.
        /// </summary>
        public static ILogger GetLogger(this ILogService logService, string category)
        {
            logService.TryGetLogger(category, out var logger);
            return logger;
        }
    }
}
#endif