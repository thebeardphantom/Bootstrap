using BeardPhantom.Bootstrap.Editor.Environment;
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    public abstract class BootstrapEditorSettingsAsset<T> : ScriptableSingleton<T>, IBootstrapEditorSettingsAsset
        where T : ScriptableObject
    {
        [field: SerializeField]
        public SettingsProperty<bool> EditorFlowEnabled { get; set; } = new(true);

        [field: SerializeField]
        public SettingsProperty<EditModeBootstrapEnvironmentAsset> EditModeEnvironment { get; private set; } = new();

        [field: SerializeField]
        public SettingsProperty<RuntimeBootstrapEnvironmentAsset> DefaultPlayModeEnvironment { get; private set; } = new();

        [field: SerializeField]
        public SettingsProperty<RuntimeBootstrapEnvironmentAsset> DefaultBuildEnvironment { get; private set; } = new();

        [field: SerializeField]
        public SettingsProperty<MappedEnvironmentCollection<SceneAsset>> EditorSceneEnvironments { get; private set; } =
            new(new MappedEnvironmentCollection<SceneAsset>());

        [field: SerializeField]
        public SettingsProperty<MappedEnvironmentCollection<string>> PlatformEnvironments { get; private set; } =
            new(new MappedEnvironmentCollection<string>());

        [field: SerializeField]
        public SettingsProperty<MappedEnvironmentCollection<BuildProfile>> BuildProfileEnvironments { get; private set; } =
            new(new MappedEnvironmentCollection<BuildProfile>());

        [field: SerializeField]
        public SettingsProperty<BootstrapLogLevel> MinLogLevel { get; private set; } = new(BootstrapLogLevel.Information);

        public void Save()
        {
            base.Save(true);
        }
    }
}