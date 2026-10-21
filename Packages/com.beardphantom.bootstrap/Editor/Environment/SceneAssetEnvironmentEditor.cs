using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Editor.Settings;
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    /// <summary>
    /// Custom inspector for <see cref="SceneAsset" /> that adds a Bootstrap environment selector mapping the
    /// scene to a <see cref="BootstrapEnvironmentAsset" />.
    /// </summary>
    [CustomEditor(typeof(SceneAsset))]
    public class SceneAssetEnvironmentEditor : UnityEditor.Editor
    {
        private ObjectField _envSelectorField;

        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            var sceneEnvironments = BootstrapEditorSettingsUtility.GetValue(
                s => s.EditorSceneEnvironments,
                out var scope);
            sceneEnvironments.TryFindEnvironmentForKey((SceneAsset)target, out var environmentAsset);

            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            _envSelectorField = new ObjectField("Bootstrap Environment")
            {
                objectType = typeof(BootstrapEnvironmentAsset),
                allowSceneObjects = false,
                value = environmentAsset,
            };
            _envSelectorField.RegisterValueChangedCallback(OnEnvSelectorValueChanged);
            root.Add(_envSelectorField);
            return root;
        }

        private void OnEnvSelectorValueChanged(ChangeEvent<Object> evt)
        {
            var sceneEnvironments = BootstrapEditorSettingsUtility.GetValue(
                s => s.EditorSceneEnvironments,
                out var _);
            sceneEnvironments.AddOrReplace((SceneAsset)target, (BootstrapEnvironmentAsset)evt.newValue);
            BootstrapSettingsProvider.SaveAllAssets();
        }
    }
}