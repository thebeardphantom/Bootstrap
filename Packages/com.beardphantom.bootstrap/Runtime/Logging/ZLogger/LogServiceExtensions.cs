#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// Extension methods for <see cref="ILogService"/>.
    /// </summary>
    public static class LogServiceExtensions
    {
        /// <summary>
        /// Attempts to get a logger for the category named after <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type whose name is used as the logger category.</typeparam>
        /// <param name="logService">The service to get the logger from.</param>
        /// <param name="logger">The resulting logger, if one could be obtained.</param>
        /// <returns>True if a logger was obtained.</returns>
        public static bool TryGetLogger<T>(this ILogService logService, out ILogger logger)
        {
            return logService.TryGetLogger(typeof(T).Name, out logger);
        }

        /// <summary>
        /// Gets a logger for the category named after <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type whose name is used as the logger category.</typeparam>
        /// <param name="logService">The service to get the logger from.</param>
        /// <returns>The logger for the category named after <typeparamref name="T"/>, or null if it could not be obtained.</returns>
        public static ILogger GetLogger<T>(this ILogService logService)
        {
            logService.TryGetLogger(typeof(T).Name, out ILogger logger);
            return logger;
        }

        /// <summary>
        /// Gets a logger for the category named after <paramref name="type"/>.
        /// </summary>
        /// <param name="logService">The service to get the logger from.</param>
        /// <param name="type">The type whose name is used as the logger category.</param>
        /// <returns>The logger for the category named after <paramref name="type"/>, or null if it could not be obtained.</returns>
        public static ILogger GetLogger(this ILogService logService, Type type)
        {
            logService.TryGetLogger(type.Name, out ILogger logger);
            return logger;
        }

        /// <summary>
        /// Gets a logger for the given <paramref name="category"/>.
        /// </summary>
        /// <param name="logService">The service to get the logger from.</param>
        /// <param name="category">The logger category name.</param>
        /// <returns>The logger for <paramref name="category"/>, or null if it could not be obtained.</returns>
        public static ILogger GetLogger(this ILogService logService, string category)
        {
            logService.TryGetLogger(category, out ILogger logger);
            return logger;
        }
    }
}
#endif