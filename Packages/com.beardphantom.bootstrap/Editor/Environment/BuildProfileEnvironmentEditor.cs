using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.Build.Profile;

namespace BeardPhantom.Bootstrap.Editor
{
    /// <summary>
    /// Draws the Bootstrap environment selector on classic and profile-based build player windows and persists
    /// the selection to editor settings.
    /// </summary>
    [InitializeOnLoad]
    public static class BuildProfileEnvironmentEditor
    {
        static BuildProfileEnvironmentEditor()
        {
            BuildProfileUtility.DrawMultiplayerBuildOptions += OnBuildPlayerWindowDraw;
        }

        private static void OnBuildPlayerWindowDraw(BuildProfile profile)
        {
            if (BuildProfileUtility.IsClassicProfile(profile))
            {
                DrawEditorForClassicBuildProfile(profile);
            }
            else
            {
                DrawEditorForBuildProfile(profile);
            }
        }

        private static void DrawEditorForClassicBuildProfile(BuildProfile profile)
        {
            var platformId = BuildProfileUtility.GetPlatformIdForProfile(profile);

            var platformEnvironments = BootstrapEditorSettingsUtility.GetValue(
                s => s.PlatformEnvironments,
                out var scope);
            platformEnvironments.TryFindEnvironmentForKey(platformId, out var environmentAsset);

            using var changeCheckScope = new EditorGUI.ChangeCheckScope();
            environmentAsset = (BootstrapEnvironmentAsset)EditorGUILayout.ObjectField(
                "Bootstrap Environment",
                environmentAsset,
                typeof(BootstrapEnvironmentAsset),
                false);
            if (changeCheckScope.changed)
            {
                platformEnvironments.AddOrReplace(platformId, environmentAsset);
                BootstrapEditorSettingsUtility.GetWithScope(scope).Save();
            }
        }

        private static void DrawEditorForBuildProfile(BuildProfile profile)
        {
            var buildProfileEnvironments = BootstrapEditorSettingsUtility.GetValue(
                s => s.BuildProfileEnvironments,
                out var scope);

            buildProfileEnvironments.TryFindEnvironmentForKey(profile, out var environmentAsset);

            using var changeCheckScope = new EditorGUI.ChangeCheckScope();
            environmentAsset = (BootstrapEnvironmentAsset)EditorGUILayout.ObjectField(
                "Bootstrap Environment",
                environmentAsset,
                typeof(BootstrapEnvironmentAsset),
                false);
            if (changeCheckScope.changed)
            {
                buildProfileEnvironments.AddOrReplace(profile, environmentAsset);
                BootstrapEditorSettingsUtility.GetWithScope(scope).Save();
            }
        }
    }
}