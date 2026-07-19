using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    /// <summary>
    /// Visual element for a <see cref="SettingsProperty{T}"/> field, pairing an override-enabled toggle with
    /// the underlying value's <see cref="UnityEditor.UIElements.PropertyField"/>.
    /// </summary>
    public class SettingsPropertyField : VisualElement
    {
        /// <summary>
        /// The toggle controlling whether this property overrides its parent value.
        /// </summary>
        public PropertyField OverrideEnabledToggle { get; }

        /// <summary>
        /// The field displaying and editing the property's value.
        /// </summary>
        public PropertyField PropertyField { get; }

        /// <summary>
        /// Parameterless constructor required for UXML instantiation.
        /// </summary>
        public SettingsPropertyField() { }

        /// <summary>
        /// Creates a field for <paramref name="property"/>, wiring up the override toggle and value field.
        /// </summary>
        /// <param name="property">The serialized <see cref="SettingsProperty{T}"/> to build UI for.</param>
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
            // I hate that I have to do this. TODO Look into GeometryChangedEvent
            var propertyField = (PropertyField)evt.target;
            propertyField.schedule.Execute(() =>
                {
                    string labelText = propertyField.label;
                    var label = propertyField.Q<Label>(className: PropertyField.labelUssClassName);
                    if (label.IsNotNull())
                    {
                        label.text = labelText;
                    }
                })
                .ExecuteLater(1);
        }

        /// <summary>
        /// Enables or disables the always-override behavior for this field.
        /// </summary>
        /// <param name="alwaysOverride">If true, hides the override toggle and always enables the value field.
        /// If false, shows the toggle and enables the value field based on its state.</param>
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