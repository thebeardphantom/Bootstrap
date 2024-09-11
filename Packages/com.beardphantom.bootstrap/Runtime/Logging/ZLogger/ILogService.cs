#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public interface ILogService
    {
        bool TryGetLogger(Type type, out ILogger logger);
    }

    public static class LogServiceExtensions
    {
        public static bool TryGetLogger<T>(this ILogService logService, out ILogger logger)
        {
            return logService.TryGetLogger(typeof(T), out logger);
        }

        public static ILogger GetLogger<T>(this ILogService logService)
        {
            logService.TryGetLogger(typeof(T), out ILogger logger);
            return logger;
        }

        public static ILogger GetLogger(this ILogService logService, Type type)
        {
            logService.TryGetLogger(type, out ILogger logger);
            return logger;
        }
    }
}
#endif