using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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

        private static async Task ExecuteTaskAsync(ScheduledTask task)
        {
            Log.Verbose($"Invoking scheduled task {task}.");
            await task.InvokeAsync();
        }

        // private uint _nextTaskId = 1;

        public void Schedule(in ScheduledTask scheduledTask)
        {
            // ScheduledTask scheduledTaskWithId = scheduledTask.WithId(_nextTaskId);
            // _nextTaskId++;
            // _tasks.Sort();
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