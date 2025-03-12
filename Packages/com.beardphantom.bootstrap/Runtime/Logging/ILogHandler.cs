using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal interface ILogHandler
    {
        void Log(LogLevel logLevel, object message, Object context = default);
    }
}