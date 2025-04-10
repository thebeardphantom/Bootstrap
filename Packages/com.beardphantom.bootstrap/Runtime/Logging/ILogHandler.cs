using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal interface ILogHandler
    {
        void Log(BootstrapLogLevel logLevel, object message, Object context = default);
    }
}