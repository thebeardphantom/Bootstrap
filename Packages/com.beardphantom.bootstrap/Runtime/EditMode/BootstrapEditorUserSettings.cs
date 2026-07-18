#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// Editor-only user-scoped Bootstrap settings, persisted to <c>UserSettings/BootstrapUserSettings.asset</c>
    /// and local to the current user's machine.
    /// </summary>
    [FilePath("UserSettings/BootstrapUserSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BootstrapEditorUserSettings : BootstrapEditorSettingsAsset<BootstrapEditorUserSettings>
    {
        /// <summary>
        /// The environment currently selected by the user for edit mode bootstrapping.
        /// </summary>
        public BootstrapEnvironmentAsset SelectedEnvironment { get; set; }
    }
}
#endif