#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public class StaticSafeLogger : ILogger
    {
        private readonly string _loggerCategory;

        private ILogger _wrappedLogger;

        private Guid? _acquisitionGuid;

        public StaticSafeLogger(string category)
        {
            _loggerCategory = category;
            ReacquireLogger(true);
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            ReacquireLogger();
            if (_wrappedLogger is NullLogger or null)
            {
                var startupLogsAppExtension = App.GetExtension<StartupLogsAppExtension>();
                string stateString = formatter.Invoke(state, exception);
                var startupLog = new StartupLog(_loggerCategory, stateString, logLevel, eventId, exception);
                startupLogsAppExtension.EnqueueStartupLog(startupLog);
            }
            else
            {
                _wrappedLogger.Log(logLevel, eventId, state, exception, formatter);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            ReacquireLogger();
            return _wrappedLogger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            ReacquireLogger();
            return _wrappedLogger.BeginScope(state);
        }

        private void ReacquireLogger(bool force = false)
        {
            if (!App.TryGetInstance(out AppInstance appInstance))
            {
                ClearWrappedLogger();
                return;
            }

            if (_acquisitionGuid.HasValue && _acquisitionGuid.Value == appInstance.SessionGuid && !force)
            {
                return;
            }

            if (ServiceRef<ILogService>.TryGetInstance(out ILogService logService)
                && logService.TryGetLogger(_loggerCategory, out _wrappedLogger))
            {
                _acquisitionGuid = appInstance.SessionGuid;
            }
            else
            {
                ClearWrappedLogger();
            }
        }

        private void ClearWrappedLogger()
        {
            _wrappedLogger = NullLogger.Instance;
            _acquisitionGuid = null;
        }
    }
}
#endif