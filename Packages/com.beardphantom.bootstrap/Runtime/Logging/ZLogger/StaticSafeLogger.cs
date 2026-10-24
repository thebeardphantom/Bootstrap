#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// <see cref="ILogger" /> wrapper safe to hold in static fields across app sessions. It lazily reacquires
    /// the underlying logger for the current <see cref="AppInstance" /> and queues log entries as
    /// <see cref="StartupLog" /> instances when no underlying logger is currently available.
    /// </summary>
    public class StaticSafeLogger : ILogger
    {
        private readonly string _loggerCategory;

        private ILogger _wrappedLogger;

        private Guid? _acquisitionGuid;

        /// <summary>
        /// Creates a new <see cref="StaticSafeLogger" /> for the given <paramref name="category" />.
        /// </summary>
        public StaticSafeLogger(string category)
        {
            _loggerCategory = category;
            ReacquireLogger(true);
        }

        /// <summary>
        /// Logs the entry via the underlying logger if one is available; otherwise queues it as a
        /// <see cref="StartupLog" /> to be replayed later.
        /// </summary>
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
                var stateString = formatter.Invoke(state, exception);
                var startupLog = new StartupLog(_loggerCategory, stateString, logLevel, eventId, exception);
                startupLogsAppExtension.EnqueueStartupLog(startupLog);
            }
            else
            {
                _wrappedLogger.Log(logLevel, eventId, state, exception, formatter);
            }
        }

        /// <summary>
        /// Checks whether the underlying logger is enabled for <paramref name="logLevel" />.
        /// </summary>
        public bool IsEnabled(LogLevel logLevel)
        {
            ReacquireLogger();
            return _wrappedLogger.IsEnabled(logLevel);
        }

        /// <summary>
        /// Begins a logging scope on the underlying logger.
        /// </summary>
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            ReacquireLogger();
            return _wrappedLogger.BeginScope(state);
        }

        private void ReacquireLogger(bool force = false)
        {
            if (!App.TryGetInstance(out var appInstance))
            {
                ClearWrappedLogger();
                return;
            }

            if (_acquisitionGuid.HasValue && _acquisitionGuid.Value == appInstance.SessionGuid && !force)
            {
                return;
            }

            if (ServiceRef<ILogService>.TryGetInstance(out var logService)
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