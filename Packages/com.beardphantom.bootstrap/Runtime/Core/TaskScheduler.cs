using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Queues <see cref="ScheduledTask" /> instances and flushes them in priority order, within a time budget.
    /// </summary>
    public class TaskScheduler
    {
        private readonly PriorityQueue<ScheduledTask, int> _tasks = new();

        private bool _isFlushingQueue;

        /// <summary>
        /// The maximum time, in milliseconds, that a single <see cref="FlushQueueAsync" /> call will spend
        /// executing queued tasks before yielding.
        /// </summary>
        public long QueueFlushTimeoutMs { get; set; } = 1000;

        /// <summary>
        /// True if no tasks are queued and no flush is currently in progress.
        /// </summary>
        public bool IsIdle => !_isFlushingQueue && _tasks.Count == 0;

        private static async Awaitable ExecuteTaskAsync(ScheduledTask task, CancellationToken cancellationToken = default)
        {
            Logging.Trace($"Invoking scheduled task {task}.");
            await task.InvokeAsync(cancellationToken);
        }

        /// <summary>
        /// Queues an asynchronous task for later execution.
        /// </summary>
        /// <param name="asyncTask">The asynchronous work to schedule.</param>
        /// <param name="priority">The priority used to order this task relative to others.</param>
        public void Schedule(in AsyncTask asyncTask, int priority = 0)
        {
            Schedule(new ScheduledTask(asyncTask, priority));
        }

        /// <summary>
        /// Queues a synchronous task for later execution.
        /// </summary>
        /// <param name="syncTask">The synchronous work to schedule.</param>
        /// <param name="priority">The priority used to order this task relative to others.</param>
        public void Schedule(in Action syncTask, int priority = 0)
        {
            Schedule(new ScheduledTask(syncTask, priority));
        }

        /// <summary>
        /// Queues an already-constructed <see cref="ScheduledTask" /> for later execution.
        /// </summary>
        /// <param name="scheduledTask">The task to schedule.</param>
        public void Schedule(in ScheduledTask scheduledTask)
        {
            _tasks.Enqueue(scheduledTask, scheduledTask.Priority);
        }

        /// <summary>
        /// Dequeues and executes queued tasks in priority order until the queue is empty or
        /// <see cref="QueueFlushTimeoutMs" /> elapses. Does nothing if a flush is already in progress.
        /// </summary>
        /// <param name="cancellationToken">A token used to cancel the flush.</param>
        public async Awaitable FlushQueueAsync(CancellationToken cancellationToken = default)
        {
            if (_isFlushingQueue)
            {
                return;
            }

            try
            {
                _isFlushingQueue = true;

                using var __ = GenericPool<Stopwatch>.Get(out var stopwatch);
                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < QueueFlushTimeoutMs && _tasks.TryDequeue(out var task, out _))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await ExecuteTaskAsync(task, cancellationToken);
                }
            }
            finally
            {
                _isFlushingQueue = false;
            }
        }
    }
}