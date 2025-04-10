#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public static class LogUtility
    {
        public static ILogger GetStaticLogger<T>()
        {
            return GetStaticLogger(typeof(T));
        }

        public static ILogger GetStaticLogger(Type type)
        {
            return new StaticSafeLogger(type.Name);
        }

        public static ILogger GetStaticLogger(string category)
        {
            return new StaticSafeLogger(category);
        }
    }
}
#endif