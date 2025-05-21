using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

namespace BeardPhantom.Bootstrap
{
    public class TaskScheduler
    {
        private readonly PriorityQueue<AsyncScheduledTask, int> _tasks = new();

        private bool _isFlushingQueue;

        public long QueueFlushTimeoutMs { get; set; } = 1000;

        public bool IsIdle => !_isFlushingQueue && _tasks.Count == 0;

        private static async Awaitable ExecuteTaskAsync(AsyncScheduledTask task)
        {
            Logging.Trace($"Invoking scheduled task {task}.");
            await task.InvokeAsync();
        }

        public void Schedule(in AsyncScheduledTask asyncScheduledTask)
        {
            _tasks.Enqueue(asyncScheduledTask, asyncScheduledTask.Priority);
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
                while (stopwatch.ElapsedMilliseconds < QueueFlushTimeoutMs && _tasks.TryDequeue(out AsyncScheduledTask task, out _))
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
