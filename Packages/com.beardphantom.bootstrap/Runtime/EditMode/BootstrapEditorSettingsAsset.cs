#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

namespace BeardPhantom.Bootstrap.EditMode
{
    public abstract class BootstrapEditorSettingsAsset<T> : ScriptableSingleton<T>, IBootstrapEditorSettingsAsset
        where T : ScriptableObject
    {
        [field: SerializeField]
        public SettingsProperty<bool> EditorFlowEnabled { get; set; } = new(true);

        [field: SerializeField]
        public SettingsProperty<GameObject> EditModeServices { get; private set; } = new();

        [field: SerializeField]
        public SettingsProperty<BootstrapEnvironmentAsset> DefaultPlayModeEnvironment { get; private set; } = new();

        [field: SerializeField]
        public SettingsProperty<BootstrapEnvironmentAsset> DefaultBuildEnvironment { get; private set; } = new();

        [field: SerializeField]
        [field: HideInInspector]
        public SettingsProperty<MappedEnvironmentCollection<SceneAsset>> EditorSceneEnvironments { get; private set; } =
            new(new MappedEnvironmentCollection<SceneAsset>());

        [field: SerializeField]
        [field: HideInInspector]
        public SettingsProperty<MappedEnvironmentCollection<string>> PlatformEnvironments { get; private set; } =
            new(new MappedEnvironmentCollection<string>());

        [field: SerializeField]
        [field: HideInInspector]
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
#endif