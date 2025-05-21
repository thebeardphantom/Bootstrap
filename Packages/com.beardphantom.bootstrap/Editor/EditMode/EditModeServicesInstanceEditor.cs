using BeardPhantom.Bootstrap.EditMode;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    [CustomEditor(typeof(EditModeServicesInstance))]
    public class EditModeServicesInstanceEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            var servicesInstance = (EditModeServicesInstance)target;
            var sourcePrefabField = new ObjectField
            {
                enabledSelf = false,
                value = servicesInstance.SourcePrefab,
            };
            root.Add(sourcePrefabField);
            return root;
        }
    }
}