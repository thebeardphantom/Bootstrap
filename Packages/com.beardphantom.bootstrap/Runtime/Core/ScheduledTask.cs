using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    public readonly struct ScheduledTask : IEquatable<ScheduledTask>, IComparable<ScheduledTask>, IComparable
    {
        internal readonly int Priority;

        private readonly Func<Awaitable> _asyncTask;

        private readonly Action _syncTask;

        public ScheduledTask(Func<Awaitable> asyncTask, int priority = 0) : this(null, asyncTask, priority) { }

        public ScheduledTask(Action syncTask, int priority = 0) : this(syncTask, null, priority) { }

        private ScheduledTask(Action syncTask, Func<Awaitable> asyncTask, int priority)
        {
            _asyncTask = asyncTask;
            _syncTask = syncTask;
            Priority = priority;
            AssertState();
        }

        public Awaitable InvokeAsync()
        {
            AssertState();
            if (_syncTask.IsNull())
            {
                return _asyncTask.Invoke();
            }

            _syncTask.Invoke();
            return AwaitableUtility.Completed;
        }

        public override string ToString()
        {
            return _syncTask.IsNull()
                ? $"{_asyncTask.Method.DeclaringType?.FullName}::{_asyncTask.Method.Name}()"
                : $"{_syncTask.Method.DeclaringType?.FullName}::{_syncTask.Method.Name}()";
        }

        public bool Equals(ScheduledTask other)
        {
            return Equals(_asyncTask, other._asyncTask)
                   && Equals(_syncTask, other._syncTask)
                   && Priority == other.Priority;
        }

        public override bool Equals(object obj)
        {
            return obj is ScheduledTask other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_syncTask, _asyncTask, Priority);
        }

        public int CompareTo(ScheduledTask other)
        {
            return Priority.CompareTo(other.Priority);
        }

        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                return 1;
            }

            return obj is ScheduledTask other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(ScheduledTask)}");
        }

        private void AssertState()
        {
            Assert.IsFalse(_syncTask.IsNull() && _asyncTask.IsNull(), "SyncTask and AsyncTask are both null.");
            Assert.IsFalse(_syncTask.IsNotNull() && _asyncTask.IsNotNull(), "SyncTask and AsyncTask both have a value.");
        }

        public static implicit operator ScheduledTask(Func<Awaitable> asyncTask)
        {
            return new ScheduledTask(asyncTask);
        }

        public static implicit operator ScheduledTask(Action syncTask)
        {
            return new ScheduledTask(syncTask);
        }

        public static bool operator ==(ScheduledTask left, ScheduledTask right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ScheduledTask left, ScheduledTask right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(ScheduledTask left, ScheduledTask right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(ScheduledTask left, ScheduledTask right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(ScheduledTask left, ScheduledTask right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(ScheduledTask left, ScheduledTask right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}