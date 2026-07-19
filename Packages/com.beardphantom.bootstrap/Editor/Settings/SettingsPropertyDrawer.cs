using BeardPhantom.Bootstrap.EditMode;
using UnityEditor;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    /// <summary>
    /// Custom property drawer for <see cref="SettingsProperty{T}"/> fields, rendering the override toggle
    /// alongside the value field via <see cref="SettingsPropertyField"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(SettingsProperty<>), true)]
    public class SettingsPropertyDrawer : PropertyDrawer
    {
        /// <inheritdoc />
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new SettingsPropertyField(property);
        }
    }
}