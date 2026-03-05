#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using UnityEditor;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    public class BootstrapSettingsProvider : SettingsProvider
    {
        public const string SettingsPath = "Project/Bootstrap";

        private SerializedObject _serializedObject;

        private BootstrapSettingsEditor _settingsEditor;

        /// <inheritdoc />
        private BootstrapSettingsProvider() : base(SettingsPath, SettingsScope.Project) { }

        public static void SaveAllAssets()
        {
            BootstrapEditorProjectSettings.instance.Save();
            BootstrapEditorUserSettings.instance.Save();
        }

        [SettingsProvider]
        private static SettingsProvider Open()
        {
            return new BootstrapSettingsProvider();
        }

        /// <inheritdoc />
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _settingsEditor = new BootstrapSettingsEditor(rootElement);
            _settingsEditor.SerializedObjectChanged += OnSettingsEditorSerializedObjectChanged;
        }

        /// <inheritdoc />
        public override void OnDeactivate()
        {
            _settingsEditor = null;
        }

        private void OnSettingsEditorSerializedObjectChanged(SerializedObject obj)
        {
            keywords = GetSearchKeywordsFromSerializedObject(obj);
        }
    }
}
#endif