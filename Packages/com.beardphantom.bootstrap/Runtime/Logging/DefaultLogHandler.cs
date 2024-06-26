using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal class DefaultLogHandler : ILogHandler
    {
        #region Methods

        /// <inheritdoc />
        public void Log(LogLevel logLevel, string tag, object message, Object context = default)
        {
            Debug.unityLogger.Log(tag, message, context);
        }

        #endregion
    }
}