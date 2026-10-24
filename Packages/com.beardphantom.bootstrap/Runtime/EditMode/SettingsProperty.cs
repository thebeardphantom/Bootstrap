#if UNITY_EDITOR
using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// A serializable settings value that can be toggled between using its own <see cref="Value" /> and deferring
    /// to a fallback (e.g. a lower-priority settings scope), and that notifies listeners when its value changes.
    /// </summary>
    [Serializable]
    public class SettingsProperty<T>
    {
        /// <summary>
        /// Whether this property's own <see cref="Value" /> should be used instead of a fallback scope's value.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Whether this property's own Value should be used instead of a fallback scope's value.")]
        public bool OverrideEnabled { get; set; }

        /// <summary>
        /// The current value of this property.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The current value of this property.")]
        public T Value { get; set; }

        /// <summary>
        /// Creates a property with the default value of <typeparamref name="T" />.
        /// </summary>
        public SettingsProperty() { }

        /// <summary>
        /// Creates a property with the given initial value.
        /// </summary>
        public SettingsProperty(T defaultValue = default)
        {
            Value = defaultValue;
        }
    }
}
#endif