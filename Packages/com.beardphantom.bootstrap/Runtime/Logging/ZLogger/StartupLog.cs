#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// A log entry captured before a real <see cref="ILogger"/> was available, to be replayed later.
    /// </summary>
    public class StartupLog
    {
        private readonly string _stateString;

        private readonly LogLevel _logLevel;

        private readonly EventId _eventId;

        private readonly Exception _exception;

        /// <summary>
        /// Gets the category of the logger that produced this entry.
        /// </summary>
        public string LoggerCategory { get; }

        /// <summary>
        /// Creates a new <see cref="StartupLog"/>.
        /// </summary>
        /// <param name="loggerCategory">The category of the logger that produced this entry.</param>
        /// <param name="stateString">The formatted log message.</param>
        /// <param name="logLevel">The severity of the entry.</param>
        /// <param name="eventId">The event id associated with the entry.</param>
        /// <param name="exception">The exception related to the entry, if any.</param>
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

        /// <summary>
        /// Replays this entry on <paramref name="logger"/>.
        /// </summary>
        /// <param name="logger">The logger to replay this entry on.</param>
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