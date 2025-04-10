#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public class StaticSafeLogger : ILogger
    {
        private static readonly ServiceRef<ILogService> s_logService = new();

        private readonly string _loggerCategory;

        private readonly Queue<IQueuedLog> _logQueue = new();

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
                _logQueue.Enqueue(new QueuedLog<TState>(logLevel, eventId, state, exception, formatter));
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
            if (_acquisitionGuid.HasValue && _acquisitionGuid.Value == App.SessionGuid && !force)
            {
                return;
            }

            if (s_logService.TryGetValue(out ILogService logService) && logService.TryGetLogger(_loggerCategory, out _wrappedLogger))
            {
                _wrappedLogger = logService.GetLogger(_loggerCategory);
                _acquisitionGuid = App.SessionGuid;
                EmptyLogQueue();
            }
            else
            {
                _wrappedLogger = NullLogger.Instance;
                _acquisitionGuid = default;
            }
        }

        private void EmptyLogQueue()
        {
            while (_logQueue.TryDequeue(out IQueuedLog queuedLog))
            {
                queuedLog.Log(_wrappedLogger);
            }
        }

        private interface IQueuedLog
        {
            void Log(ILogger logger);
        }

        private class QueuedLog<TState> : IQueuedLog
        {
            private readonly LogLevel _logLevel;

            private readonly EventId _eventId;

            private readonly TState _state;

            private readonly Exception _exception;

            private readonly Func<TState, Exception, string> _formatter;

            public QueuedLog(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception exception,
                Func<TState, Exception, string> formatter)
            {
                _logLevel = logLevel;
                _eventId = eventId;
                _state = state;
                _exception = exception;
                _formatter = formatter;
            }

            public void Log(ILogger logger)
            {
                logger.Log(_logLevel, _eventId, _state, _exception, _formatter);
            }
        }
    }
}
#endif