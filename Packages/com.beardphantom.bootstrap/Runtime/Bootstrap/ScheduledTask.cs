using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public readonly struct ScheduledTask : IComparable<ScheduledTask>, IEquatable<ScheduledTask>
    {
        internal readonly int Priority;

        private readonly Func<Awaitable> _task;

        public ScheduledTask(Func<Awaitable> task, int priority = 0) : this(task, priority, 0) { }

        private ScheduledTask(Func<Awaitable> task, int priority, uint id)
        {
            _task = task;
            Priority = priority;
        }

        public Awaitable InvokeAsync()
        {
            return _task.Invoke();
        }

        public override string ToString()
        {
            return $"{_task.Method.DeclaringType?.FullName}::{_task.Method.Name}()";
        }

        public int CompareTo(ScheduledTask other)
        {
            return Priority.CompareTo(other.Priority);
        }

        public bool Equals(ScheduledTask other)
        {
            return Equals(_task, other._task)
                   && Priority == other.Priority;
        }

        public override bool Equals(object obj)
        {
            return obj is ScheduledTask other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_task, Priority);
        }

        public static bool operator ==(ScheduledTask left, ScheduledTask right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ScheduledTask left, ScheduledTask right)
        {
            return !left.Equals(right);
        }

        public static implicit operator ScheduledTask(Func<Awaitable> task)
        {
            return new ScheduledTask(task);
        }
    }
}