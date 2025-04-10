using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

namespace BeardPhantom.Bootstrap
{
    public class AsyncTaskScheduler
    {
        private readonly PriorityQueue<ScheduledTask, int> _tasks = new();

        private bool _isFlushingQueue;

        public long QueueFlushTimeoutMs { get; set; } = 1000;

        public bool IsIdle => !_isFlushingQueue && _tasks.Count == 0;

        private static async Awaitable ExecuteTaskAsync(ScheduledTask task)
        {
            Logging.Trace($"Invoking scheduled task {task}.");
            await task.InvokeAsync();
        }

        public void Schedule(in ScheduledTask scheduledTask)
        {
            _tasks.Enqueue(scheduledTask, scheduledTask.Priority);
        }

        public async Awaitable FlushQueueAsync(CancellationToken token = default)
        {
            if (_isFlushingQueue)
            {
                return;
            }

            try
            {
                _isFlushingQueue = true;

                using PooledObject<Stopwatch> __ = GenericPool<Stopwatch>.Get(out Stopwatch stopwatch);
                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < QueueFlushTimeoutMs && _tasks.TryDequeue(out ScheduledTask task, out _))
                {
                    token.ThrowIfCancellationRequested();
                    await ExecuteTaskAsync(task);
                }
            }
            finally
            {
                _isFlushingQueue = false;
            }
        }
    }
}
