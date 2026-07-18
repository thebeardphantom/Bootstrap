#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// <see cref="ILogger"/> wrapper safe to hold in static fields across app sessions. It lazily reacquires
    /// the underlying logger for the current <see cref="AppInstance"/> and queues log entries as
    /// <see cref="StartupLog"/> instances when no underlying logger is currently available.
    /// </summary>
    public class StaticSafeLogger : ILogger
    {
        private readonly string _loggerCategory;

        private ILogger _wrappedLogger;

        private Guid? _acquisitionGuid;

        /// <summary>
        /// Creates a new <see cref="StaticSafeLogger"/> for the given <paramref name="category"/>.
        /// </summary>
        /// <param name="category">The logger category name.</param>
        public StaticSafeLogger(string category)
        {
            _loggerCategory = category;
            ReacquireLogger(true);
        }

        /// <summary>
        /// Logs the entry via the underlying logger if one is available; otherwise queues it as a
        /// <see cref="StartupLog"/> to be replayed later.
        /// </summary>
        /// <typeparam name="TState">The type of the object to be logged.</typeparam>
        /// <param name="logLevel">The severity of the log entry.</param>
        /// <param name="eventId">The event id associated with the log entry.</param>
        /// <param name="state">The entry to be logged.</param>
        /// <param name="exception">The exception related to the entry, if any.</param>
        /// <param name="formatter">The function used to format <paramref name="state"/> and <paramref name="exception"/> into a message.</param>
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

        /// <summary>
        /// Checks whether the underlying logger is enabled for <paramref name="logLevel"/>.
        /// </summary>
        /// <param name="logLevel">The level to check.</param>
        public bool IsEnabled(LogLevel logLevel)
        {
            ReacquireLogger();
            return _wrappedLogger.IsEnabled(logLevel);
        }

        /// <summary>
        /// Begins a logging scope on the underlying logger.
        /// </summary>
        /// <typeparam name="TState">The type of the state object.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>A disposable that ends the scope when disposed.</returns>
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