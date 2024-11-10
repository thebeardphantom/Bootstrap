using BeardPhantom.Bootstrap.Editor.Settings;
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Environment
{
    [CustomEditor(typeof(SceneAsset))]
    public class SceneAssetEnvironmentEditor : UnityEditor.Editor
    {
        private ObjectField _envSelectorField;

        public override VisualElement CreateInspectorGUI()
        {
            MappedEnvironmentCollection<SceneAsset> sceneEnvironments = BootstrapEditorSettingsUtility.GetValue(
                s => s.EditorSceneEnvironments,
                out SettingsScope scope);
            sceneEnvironments.TryFindEnvironmentForKey((SceneAsset)target, out RuntimeBootstrapEnvironmentAsset environmentAsset);

            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            _envSelectorField = new ObjectField("Bootstrap Environment")
            {
                objectType = typeof(RuntimeBootstrapEnvironmentAsset),
                allowSceneObjects = false,
                value = environmentAsset,
            };
            _envSelectorField.RegisterValueChangedCallback(OnEnvSelectorValueChanged);
            root.Add(_envSelectorField);
            return root;
        }

        private void OnEnvSelectorValueChanged(ChangeEvent<Object> evt)
        {
            MappedEnvironmentCollection<SceneAsset> buildProfileEnvironments = BootstrapEditorSettingsUtility.GetValue(
                s => s.EditorSceneEnvironments,
                out SettingsScope _);
            buildProfileEnvironments.AddOrReplace((SceneAsset)target, (RuntimeBootstrapEnvironmentAsset)evt.newValue);
        }
    }
}