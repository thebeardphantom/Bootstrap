#if BOOTSTRAP_ZLOGGER
using BeardPhantom.Bootstrap;
using BeardPhantom.Bootstrap.ZLogger;
using System.Collections.Concurrent;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

[assembly: AppExtensionType(typeof(StartupLogsAppExtension))]

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// App extension that queues <see cref="StartupLog"/> entries raised before an <see cref="ILogService"/>
    /// is available, then flushes them once one is ready.
    /// </summary>
    public class StartupLogsAppExtension : IAppExtension
    {
        private readonly ConcurrentQueue<StartupLog> _startupLogQueue = new();

        private bool _hasFlushedStartupLogs;

        /// <summary>
        /// Gets whether there are queued startup logs that have not yet been flushed.
        /// </summary>
        public bool HasStartupLogs => !_startupLogQueue.IsEmpty;

        /// <summary>
        /// Creates a new <see cref="StartupLogsAppExtension"/> and subscribes to <see cref="App.AppInstanceDestroyed"/>
        /// to reset flush state when the app deinitializes.
        /// </summary>
        public StartupLogsAppExtension()
        {
            App.AppInstanceDestroyed += OnAppInstanceDestroyed;
        }

        /// <summary>
        /// Replays and clears all queued startup logs using <paramref name="logService"/>. Does nothing if
        /// already flushed since the last app deinitialization.
        /// </summary>
        /// <param name="logService">The service used to resolve loggers for queued entries.</param>
        [HideInCallstack]
        public void Flush(ILogService logService)
        {
            if (_hasFlushedStartupLogs)
            {
                return;
            }

            _hasFlushedStartupLogs = true;
            while (!_startupLogQueue.IsEmpty)
            {
                while (_startupLogQueue.TryDequeue(out StartupLog queuedLog))
                {
                    string loggerCategory = queuedLog.LoggerCategory;
                    if (logService.TryGetLogger(loggerCategory, out ILogger logger))
                    {
                        queuedLog.Log(logger);
                    }
                    else
                    {
                        Debug.LogError($"Logger not found: {loggerCategory}");
                    }
                }
            }
        }

        /// <summary>
        /// Queues <paramref name="startupLog"/> to be replayed the next time <see cref="Flush"/> is called.
        /// </summary>
        /// <param name="startupLog">The log entry to queue.</param>
        public void EnqueueStartupLog(StartupLog startupLog)
        {
            _startupLogQueue.Enqueue(startupLog);
        }

        private void OnAppInstanceDestroyed()
        {
            _hasFlushedStartupLogs = false;
        }
    }
}
#endif