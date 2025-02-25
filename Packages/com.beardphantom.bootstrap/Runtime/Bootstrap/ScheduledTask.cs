using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public readonly struct ScheduledTask : IComparable<ScheduledTask>, IEquatable<ScheduledTask>
    {
        internal readonly int Priority;

        private readonly Func<Awaitable> _task;

        // private readonly uint _id;

        public ScheduledTask(Func<Awaitable> task, int priority = 0) : this(task, priority, 0) { }

        private ScheduledTask(Func<Awaitable> task, int priority, uint id)
        {
            _task = task;
            Priority = priority;
            // _id = id;
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
            // return priorityComparison == 0 ? _id.CompareTo(other._id) : priorityComparison;
        }

        public bool Equals(ScheduledTask other)
        {
            return Equals(_task, other._task)
                   && Priority == other.Priority;
            // && _id == other._id;
        }

        public override bool Equals(object obj)
        {
            return obj is ScheduledTask other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_task, Priority);
        }

        // internal ScheduledTask WithId(uint id)
        // {
        //     return new ScheduledTask(_task, Priority, id);
        // }

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