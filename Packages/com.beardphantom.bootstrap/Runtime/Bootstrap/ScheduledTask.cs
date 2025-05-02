using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public readonly struct ScheduledTask : IEquatable<ScheduledTask>, IComparable<ScheduledTask>, IComparable
    {
        internal readonly int Priority;

        private readonly Func<Awaitable> _task;

        public ScheduledTask(Func<Awaitable> task, int priority = 0)
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

        public static implicit operator ScheduledTask(Func<Awaitable> task)
        {
            return new ScheduledTask(task);
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