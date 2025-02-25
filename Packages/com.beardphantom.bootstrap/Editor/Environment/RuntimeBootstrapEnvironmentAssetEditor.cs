using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Environment
{
    [CustomEditor(typeof(RuntimeBootstrapEnvironmentAsset))]
    public class RuntimeBootstrapEnvironmentAssetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            root.Add(
                new HelpBox(
                    $"This {nameof(RuntimeBootstrapEnvironmentAsset)} has no assigned Bootstrapper Prefab. It will act as an environment that performs no bootstrapping and provides no services.",
                    HelpBoxMessageType.Info));
            PropertyField objectField = root.Query<PropertyField>()
                .Where(pf => pf.bindingPath.EndsWith("<BootstrapperPrefab>k__BackingField"))
                .First();
            objectField.RegisterValueChangeCallback(OnBootstrapperPrefabValueChanged);
            return root;
        }

        private void OnBootstrapperPrefabValueChanged(SerializedPropertyChangeEvent evt) { }
    }
}