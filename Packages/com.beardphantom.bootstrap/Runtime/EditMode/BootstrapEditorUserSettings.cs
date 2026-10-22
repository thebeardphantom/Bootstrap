#if UNITY_EDITOR
using UnityEditor;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// Editor-only user-scoped Bootstrap settings, persisted to <c>UserSettings/BootstrapUserSettings.asset</c>
    /// and local to the current user's machine.
    /// </summary>
    [FilePath("UserSettings/BootstrapUserSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BootstrapEditorUserSettings : BootstrapEditorSettingsAsset<BootstrapEditorUserSettings> { }
}
#endif