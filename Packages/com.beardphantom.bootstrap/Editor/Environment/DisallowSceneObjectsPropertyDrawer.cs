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
            propertyField.schedule.Execute(
                    () =>
                    {
                        propertyField.Q<ObjectField>().allowSceneObjects = false;
                    })
                .StartingIn(50);
            return propertyField;
        }
    }
}