using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    [Serializable]
    public class SettingsProperty<T>
    {
        public event Action<T> ValueChanged;

        [field: SerializeField]
        public bool OverrideEnabled { get; set; }

        [field: SerializeField]
        public T Value { get; private set; }

        public SettingsProperty() { }

        public SettingsProperty(T defaultValue = default) { }

        public void SetValue(T newValue)
        {
            Value = newValue;
            ValueChanged?.Invoke(newValue);
        }
    }
}