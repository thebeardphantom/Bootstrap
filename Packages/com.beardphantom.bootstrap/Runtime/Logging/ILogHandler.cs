using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal interface ILogHandler
    {
        void Log(LogLevel logLevel, string tag, object message, Object context = default);
    }
}