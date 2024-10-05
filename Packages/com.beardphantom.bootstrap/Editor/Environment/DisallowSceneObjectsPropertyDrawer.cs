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
            var objectField = new PropertyField(property);
            objectField.schedule.Execute(
                    () =>
                    {
                        objectField.Q<ObjectField>().allowSceneObjects = false;
                    })
                .ExecuteLater(10);
            return objectField;
        }
    }
}