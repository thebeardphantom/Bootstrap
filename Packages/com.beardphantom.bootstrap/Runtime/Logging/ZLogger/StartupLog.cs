#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public class StartupLog
    {
        private readonly string _stateString;

        private readonly LogLevel _logLevel;

        private readonly EventId _eventId;

        private readonly Exception _exception;

        public string LoggerCategory { get; }

        public StartupLog(
            string loggerCategory,
            string stateString,
            LogLevel logLevel,
            EventId eventId,
            Exception exception)
        {
            LoggerCategory = loggerCategory;
            _stateString = stateString;
            _logLevel = logLevel;
            _eventId = eventId;
            _exception = exception;
        }

        [HideInCallstack]
        public void Log(ILogger logger)
        {
#pragma warning disable CA2254
            logger.Log(_logLevel, _eventId, _exception, _stateString);
#pragma warning restore CA2254
        }
    }
}
#endif