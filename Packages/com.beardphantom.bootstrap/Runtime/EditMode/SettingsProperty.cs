#if UNITY_EDITOR
using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap.EditMode
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

        public SettingsProperty(T defaultValue = default)
        {
            Value = defaultValue;
        }

        public void SetValue(T newValue)
        {
            Value = newValue;
            ValueChanged?.Invoke(newValue);
        }
    }
}
#endif