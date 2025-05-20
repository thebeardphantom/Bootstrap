using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    [FilePath("ProjectSettings/BootstrapProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BootstrapEditorProjectSettings : BootstrapEditorSettingsAsset<BootstrapEditorProjectSettings>,
        ISerializationCallbackReceiver
    {
        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            EditorFlowEnabled.OverrideEnabled = true;
            EditModeServices.OverrideEnabled = true;
            DefaultPlayModeEnvironment.OverrideEnabled = true;
            DefaultBuildEnvironment.OverrideEnabled = true;
            PlatformEnvironments.OverrideEnabled = true;
            EditorSceneEnvironments.OverrideEnabled = true;
            BuildProfileEnvironments.OverrideEnabled = true;
            MinLogLevel.OverrideEnabled = true;
        }
    }
}