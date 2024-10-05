using BeardPhantom.Bootstrap.Environment;
using UnityEditor;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    [FilePath("UserSettings/BootstrapUserSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BootstrapEditorUserSettings : BootstrapEditorSettingsAsset<BootstrapEditorUserSettings>
    {
        public BootstrapEnvironmentAsset SelectedEnvironment { get; set; }
    }
}