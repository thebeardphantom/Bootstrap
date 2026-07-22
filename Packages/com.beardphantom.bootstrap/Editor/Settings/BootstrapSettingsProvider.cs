#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using UnityEditor;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    /// <summary>
    /// Project Settings page for editing <see cref="BootstrapEditorProjectSettings"/> and
    /// <see cref="BootstrapEditorUserSettings"/>.
    /// </summary>
    public class BootstrapSettingsProvider : SettingsProvider
    {
        /// <summary>
        /// The Project Settings menu path this provider is registered under.
        /// </summary>
        public const string SettingsPath = "Project/Bootstrap";

        private SerializedObject _serializedObject;

        private BootstrapSettingsEditor _settingsEditor;

        /// <inheritdoc />
        private BootstrapSettingsProvider() : base(SettingsPath, SettingsScope.Project) { }

        /// <summary>
        /// Saves both the project-scoped and user-scoped bootstrap editor settings assets.
        /// </summary>
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
            _settingsEditor?.Dispose();
            _settingsEditor = null;
        }

        private void OnSettingsEditorSerializedObjectChanged(SerializedObject obj)
        {
            keywords = GetSearchKeywordsFromSerializedObject(obj);
        }
    }
}
#endif