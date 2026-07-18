#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.Build.Profile;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// Common contract implemented by Bootstrap's editor settings assets (project- and user-scoped), exposing
    /// the settings values consumed by <see cref="BootstrapEditorSettingsUtility"/>.
    /// </summary>
    public interface IBootstrapEditorSettingsAsset
    {
        /// <summary>
        /// Whether Bootstrap's editor play mode flow is enabled.
        /// </summary>
        SettingsProperty<bool> EditorFlowEnabled { get; set; }

        /// <summary>
        /// The service list used to bootstrap edit mode services.
        /// </summary>
        SettingsProperty<ServiceList> EditModeServices { get; }

        /// <summary>
        /// The environment used by default when entering play mode.
        /// </summary>
        SettingsProperty<BootstrapEnvironmentAsset> DefaultPlayModeEnvironment { get; }

        /// <summary>
        /// The environment used by default for builds.
        /// </summary>
        SettingsProperty<BootstrapEnvironmentAsset> DefaultBuildEnvironment { get; }

        /// <summary>
        /// Per-platform environment overrides, keyed by platform name.
        /// </summary>
        SettingsProperty<MappedEnvironmentCollection<string>> PlatformEnvironments { get; }

        /// <summary>
        /// Per-scene environment overrides, keyed by scene asset.
        /// </summary>
        SettingsProperty<MappedEnvironmentCollection<SceneAsset>> EditorSceneEnvironments { get; }

        /// <summary>
        /// Per-build-profile environment overrides, keyed by build profile.
        /// </summary>
        SettingsProperty<MappedEnvironmentCollection<BuildProfile>> BuildProfileEnvironments { get; }

        /// <summary>
        /// The minimum log level Bootstrap will emit.
        /// </summary>
        SettingsProperty<BootstrapLogLevel> MinLogLevel { get; }

        /// <summary>
        /// Persists this settings asset to disk.
        /// </summary>
        void Save();
    }
}
#endif