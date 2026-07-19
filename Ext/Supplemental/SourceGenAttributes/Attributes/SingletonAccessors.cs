using System;

namespace BeardPhantom.Bootstrap.SourceGen
{
    /// <summary>
    /// Flags controlling which accessors <see cref="GenerateSingletonAttribute"/> generates.
    /// </summary>
    [Flags]
    public enum SingletonAccessors
    {
        /// <summary>
        /// Generates a property of the Service type.
        /// </summary>
        Property = 1 << 0,

        /// <summary>
        /// Generates a method with an out parameter of the Service type.
        /// </summary>
        OutMethod = 1 << 1,
    }

    /// <summary>
    /// Extension methods for <see cref="SingletonAccessors"/>.
    /// </summary>
    public static class SingletonAccessorsExtensions
    {
        /// <summary>
        /// Checks whether <paramref name="value"/> has <paramref name="flag"/> set, without the boxing allocation
        /// of <see cref="Enum.HasFlag"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="flag">The flag to check for.</param>
        public static bool HasFlagFast(this SingletonAccessors value, SingletonAccessors flag)
        {
            return (value & flag) != 0;
        }
    }
}