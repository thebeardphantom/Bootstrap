using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal interface ILogHandler
    {
        void Log(in BootstrapLogLevel logLevel, object message, Object context = default);
    }
}