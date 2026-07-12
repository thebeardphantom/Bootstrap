using System;

namespace BeardPhantom.Bootstrap.SourceGen
{
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

    public static class SingletonAccessorsExtensions
    {
        public static bool HasFlagFast(this SingletonAccessors value, SingletonAccessors flag)
        {
            return (value & flag) != 0;
        }
    }
}