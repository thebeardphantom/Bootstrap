#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// Editor-only project-scoped Bootstrap settings, persisted to
    /// <c>ProjectSettings/BootstrapProjectSettings.asset</c> and shared across all users of the project.
    /// </summary>
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
#endif