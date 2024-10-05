using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    public class SettingsPropertyField : VisualElement
    {
        public PropertyField OverrideEnabledToggle { get; }

        public PropertyField PropertyField { get; }

        public SettingsPropertyField() { }

        public SettingsPropertyField(SerializedProperty property)
        {
            style.flexDirection = FlexDirection.Row;
            SerializedProperty overrideEnabledProperty = property.FindPropertyRelative(
                "OverrideEnabled".GetSerializedBackingFieldName());
            OverrideEnabledToggle = new PropertyField(overrideEnabledProperty, "");
            Add(OverrideEnabledToggle);

            SerializedProperty valueProperty = property.FindPropertyRelative(
                "Value".GetSerializedBackingFieldName());
            PropertyField = new PropertyField(valueProperty, property.displayName)
            {
                style =
                {
                    flexGrow = 1,
                },
            };
            PropertyField.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            Add(PropertyField);

            SetAlwaysOverride(false);
            PropertyField.SetEnabled(overrideEnabledProperty.boolValue);
        }

        private static void OnAttachToPanel(AttachToPanelEvent evt)
        {
            // I hate that I have to do this.
            var propertyField = (PropertyField)evt.target;
            propertyField.schedule.Execute(
                    () =>
                    {
                        string labelText = propertyField.label;
                        var label = propertyField.Q<Label>(className: PropertyField.labelUssClassName);
                        if (label != null)
                        {
                            label.text = labelText;
                        }
                    })
                .ExecuteLater(1);
        }

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
    }
}