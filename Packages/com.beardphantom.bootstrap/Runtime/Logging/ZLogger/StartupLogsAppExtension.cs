#if BOOTSTRAP_ZLOGGER
using BeardPhantom.Bootstrap;
using BeardPhantom.Bootstrap.ZLogger;
using System.Collections.Concurrent;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

[assembly: AppExtensionType(typeof(StartupLogsAppExtension))]

namespace BeardPhantom.Bootstrap.ZLogger
{
    public class StartupLogsAppExtension : IAppExtension
    {
        private readonly ConcurrentQueue<StartupLog> _startupLogQueue = new();

        private bool _hasFlushedStartupLogs;

        public bool HasStartupLogs => !_startupLogQueue.IsEmpty;

        public StartupLogsAppExtension()
        {
            App.Deinitialized += OnAppDeinitialized;
        }

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

        public void EnqueueStartupLog(StartupLog startupLog)
        {
            _startupLogQueue.Enqueue(startupLog);
        }

        private void OnAppDeinitialized()
        {
            _hasFlushedStartupLogs = false;
        }
    }
}
#endif