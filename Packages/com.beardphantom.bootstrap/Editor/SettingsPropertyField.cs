using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    public class SettingsPropertyField : VisualElement
    {
        #region Properties

        public PropertyField OverrideEnabledToggle { get; }

        public PropertyField PropertyField { get; }

        #endregion

        #region Constructors

        public SettingsPropertyField() { }

        public SettingsPropertyField(SerializedProperty property)
        {
            style.flexDirection = FlexDirection.Row;
            var overrideEnabledProperty = property.FindPropertyRelative("<OverrideEnabled>k__BackingField");
            OverrideEnabledToggle = new PropertyField(overrideEnabledProperty, "");
            Add(OverrideEnabledToggle);

            var valueProperty = property.FindPropertyRelative("<Value>k__BackingField");
            PropertyField = new PropertyField(valueProperty, property.displayName)
            {
                style =
                {
                    flexGrow = 1
                }
            };
            Add(PropertyField);

            SetAlwaysOverride(false);
            PropertyField.SetEnabled(overrideEnabledProperty.boolValue);
        }

        #endregion

        #region Methods

        public void SetAlwaysOverride(bool alwaysOverride)
        {
            if (alwaysOverride)
            {
                OverrideEnabledToggle.UnregisterCallback<SerializedPropertyChangeEvent>(OnOverrideValueChanged);
                OverrideEnabledToggle.style.display = DisplayStyle.None;
                PropertyField.SetEnabled(true);
            }
            else
            {
                OverrideEnabledToggle.style.display = DisplayStyle.Flex;
                OverrideEnabledToggle.RegisterCallback<SerializedPropertyChangeEvent>(OnOverrideValueChanged);
            }
        }

        private void OnOverrideValueChanged(SerializedPropertyChangeEvent evt)
        {
            PropertyField.SetEnabled(evt.changedProperty.boolValue);
        }

        #endregion
    }
}