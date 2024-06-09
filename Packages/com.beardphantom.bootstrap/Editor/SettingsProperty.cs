using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    [Serializable]
    public class SettingsProperty<T>
    {
        #region Events

        public event Action<T> ValueChanged;

        #endregion

        #region Properties

        [field: SerializeField]
        public bool OverrideEnabled { get; set; }

        [field: SerializeField]
        public T Value { get; private set; }

        #endregion

        #region Constructors

        public SettingsProperty() { }

        public SettingsProperty(T defaultValue = default) { }

        #endregion

        #region Methods

        public void SetValue(T newValue)
        {
            Value = newValue;
            ValueChanged?.Invoke(newValue);
        }

        #endregion
    }
}