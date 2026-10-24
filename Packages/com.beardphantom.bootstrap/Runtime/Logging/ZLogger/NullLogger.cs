using Microsoft.Extensions.Logging;
using System;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// No-op <see cref="ILogger" /> implementation used as a placeholder when no real logger is available.
    /// </summary>
    public class NullLogger : ILogger, IDisposable
    {
        /// <summary>
        /// The singleton <see cref="NullLogger" /> instance.
        /// </summary>
        public static readonly NullLogger Instance = new();

        private NullLogger() { }

        /// <summary>
        /// Does nothing; this logger discards all log entries.
        /// </summary>
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter) { }

        /// <summary>
        /// Always returns true.
        /// </summary>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <summary>
        /// Returns this instance; no scope tracking is performed.
        /// </summary>
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return this;
        }

        public void Dispose() { }
    }
}