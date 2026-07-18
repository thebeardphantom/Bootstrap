#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// Base class for Bootstrap's editor settings assets, backed by Unity's <see cref="ScriptableSingleton{T}"/>
    /// and storing the values defined by <see cref="IBootstrapEditorSettingsAsset"/>.
    /// </summary>
    /// <typeparam name="T">The concrete settings asset type.</typeparam>
    public abstract class BootstrapEditorSettingsAsset<T> : ScriptableSingleton<T>, IBootstrapEditorSettingsAsset
        where T : ScriptableObject
    {
        /// <inheritdoc />
        [field: SerializeField]
        [field: Tooltip("Whether Bootstrap's editor play mode flow is enabled.")]
        public SettingsProperty<bool> EditorFlowEnabled { get; set; } = new(true);

        /// <inheritdoc />
        [field: SerializeField]
        [field: Tooltip("The service list used to bootstrap edit mode services.")]
        public SettingsProperty<ServiceList> EditModeServices { get; private set; } = new();

        /// <inheritdoc />
        [field: SerializeField]
        [field: Tooltip("The environment used by default when entering play mode.")]
        public SettingsProperty<BootstrapEnvironmentAsset> DefaultPlayModeEnvironment { get; private set; } = new();

        /// <inheritdoc />
        [field: SerializeField]
        [field: Tooltip("The environment used by default for builds.")]
        public SettingsProperty<BootstrapEnvironmentAsset> DefaultBuildEnvironment { get; private set; } = new();

        /// <inheritdoc />
        [field: SerializeField]
        [field: Tooltip("Per-scene environment overrides, keyed by scene asset.")]
        public SettingsProperty<MappedEnvironmentCollection<SceneAsset>> EditorSceneEnvironments { get; private set; } =
            new(new MappedEnvironmentCollection<SceneAsset>());

        /// <inheritdoc />
        [field: SerializeField]
        [field: Tooltip("Per-platform environment overrides, keyed by platform name.")]
        public SettingsProperty<MappedEnvironmentCollection<string>> PlatformEnvironments { get; private set; } =
            new(new MappedEnvironmentCollection<string>());

        /// <inheritdoc />
        [field: SerializeField]
        [field: Tooltip("Per-build-profile environment overrides, keyed by build profile.")]
        public SettingsProperty<MappedEnvironmentCollection<BuildProfile>> BuildProfileEnvironments { get; private set; } =
            new(new MappedEnvironmentCollection<BuildProfile>());

        /// <inheritdoc />
        [field: SerializeField]
        [field: Tooltip("The minimum log level Bootstrap will emit.")]
        public SettingsProperty<BootstrapLogLevel> MinLogLevel { get; private set; } = new(BootstrapLogLevel.Information);

        /// <inheritdoc />
        public void Save()
        {
            base.Save(true);
        }
    }
}
#endif