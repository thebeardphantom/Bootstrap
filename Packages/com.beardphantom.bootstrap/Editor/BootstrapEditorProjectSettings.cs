using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
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
            MinLogLevel.OverrideEnabled = true;
        }
    }
}