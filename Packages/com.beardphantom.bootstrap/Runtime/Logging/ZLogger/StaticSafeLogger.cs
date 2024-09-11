#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public class StaticSafeLogger : ILogger
    {
        private static readonly ServiceRef<ILogService> s_logService = new();

        private readonly Type _loggerType;

        private ILogger _wrappedLogger;

        private Guid? _acquisitionGuid;

        public StaticSafeLogger(Type loggerType)
        {
            _loggerType = loggerType;
            ReacquireLogger(true);
        }

        public void Log<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            ReacquireLogger();
            _wrappedLogger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
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
            if (_acquisitionGuid.HasValue && _acquisitionGuid.Value == App.SessionGuid && !force)
            {
                return;
            }

            if (s_logService.TryGetValue(out ILogService logService) && logService.TryGetLogger(_loggerType, out _wrappedLogger))
            {
                _wrappedLogger = logService.GetLogger(_loggerType);
                _acquisitionGuid = App.SessionGuid;
            }
            else
            {
                _wrappedLogger = NullLogger.Instance;
                _acquisitionGuid = default;
            }
        }
    }
}
#endif