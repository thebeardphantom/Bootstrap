using UnityEngine;

namespace BeardPhantom.Bootstrap.Logging
{
    public interface ILogHandler
    {
        #region Methods

        void Log(LogLevel logLevel, string tag, object message, Object context = default);

        #endregion
    }
}