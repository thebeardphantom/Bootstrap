using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Environment
{
    [CustomPropertyDrawer(typeof(DisallowSceneObjectsAttribute))]
    public class DisallowSceneObjectsPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyField = new PropertyField(property);
            propertyField.RegisterCallbackOnce<GeometryChangedEvent>(_ =>
            {
                propertyField.Q<ObjectField>().allowSceneObjects = false;
            });
            return propertyField;
        }
    }
}