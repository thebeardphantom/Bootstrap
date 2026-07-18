using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Marks a serialized field as selectable from any concrete subtype of <see cref="BaseType"/>, allowing the
    /// inspector to present a type picker for polymorphic serialization.
    /// </summary>
    public class PolymorphicTypeSelectorAttribute : PropertyAttribute
    {
        /// <summary>
        /// The base type that selectable types must derive from or implement.
        /// </summary>
        public readonly Type BaseType;

        /// <summary>
        /// Creates a new polymorphic type selector attribute.
        /// </summary>
        /// <param name="baseType">The base type that selectable types must derive from or implement.</param>
        public PolymorphicTypeSelectorAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}