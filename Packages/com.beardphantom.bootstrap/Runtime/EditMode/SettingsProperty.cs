#if UNITY_EDITOR
using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// A serializable settings value that can be toggled between using its own <see cref="Value"/> and deferring
    /// to a fallback (e.g. a lower-priority settings scope), and that notifies listeners when its value changes.
    /// </summary>
    /// <typeparam name="T">The type of the settings value.</typeparam>
    [Serializable]
    public class SettingsProperty<T>
    {
        /// <summary>
        /// Raised after <see cref="Value"/> changes via <see cref="SetValue"/>.
        /// </summary>
        public event Action<T> ValueChanged;

        /// <summary>
        /// Whether this property's own <see cref="Value"/> should be used instead of a fallback scope's value.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Whether this property's own Value should be used instead of a fallback scope's value.")]
        public bool OverrideEnabled { get; set; }

        /// <summary>
        /// The current value of this property.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The current value of this property.")]
        public T Value { get; private set; }

        /// <summary>
        /// Creates a property with the default value of <typeparamref name="T"/>.
        /// </summary>
        public SettingsProperty() { }

        /// <summary>
        /// Creates a property with the given initial value.
        /// </summary>
        /// <param name="defaultValue">The initial value of the property.</param>
        public SettingsProperty(T defaultValue = default)
        {
            Value = defaultValue;
        }

        /// <summary>
        /// Sets <see cref="Value"/> and raises <see cref="ValueChanged"/>.
        /// </summary>
        /// <param name="newValue">The new value to assign.</param>
        public void SetValue(T newValue)
        {
            Value = newValue;
            ValueChanged?.Invoke(newValue);
        }
    }
}
#endif