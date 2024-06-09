using UnityEditor;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    [CustomPropertyDrawer(typeof(SettingsProperty<>))]
    public class SettingsPropertyPropertyDrawer : PropertyDrawer
    {
        #region Methods

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new SettingsPropertyField(property);
        }

        #endregion
    }
}