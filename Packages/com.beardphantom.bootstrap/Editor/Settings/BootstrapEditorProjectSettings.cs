using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    [FilePath("ProjectSettings/BootstrapProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BootstrapEditorProjectSettings : BootstrapEditorSettingsAsset<BootstrapEditorProjectSettings>,
        ISerializationCallbackReceiver
    {
        [field: SerializeField]
        public bool VerboseLogging { get; private set; }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            EditorFlowEnabled.OverrideEnabled = true;
            EditModeEnvironment.OverrideEnabled = true;
            EditorSceneEnvironments.OverrideEnabled = true;
            BuildProfileEnvironments.OverrideEnabled = true;
            PlatformEnvironments.OverrideEnabled = true;
        }
    }
}