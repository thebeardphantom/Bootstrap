using UnityEditor;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    [CustomPropertyDrawer(typeof(SettingsProperty<>), true)]
    public class SettingsPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new SettingsPropertyField(property);
        }
    }
}