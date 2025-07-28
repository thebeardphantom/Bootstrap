using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.Build.Profile;

namespace BeardPhantom.Bootstrap.Editor.Environment
{
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
            string platformId = BuildProfileUtility.GetPlatformIdForProfile(profile);

            MappedEnvironmentCollection<string> platformEnvironments = BootstrapEditorSettingsUtility.GetValue(
                s => s.PlatformEnvironments,
                out SettingsScope scope);
            platformEnvironments.TryFindEnvironmentForKey(platformId, out BootstrapEnvironmentAsset environmentAsset);

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
            MappedEnvironmentCollection<BuildProfile> buildProfileEnvironments = BootstrapEditorSettingsUtility.GetValue(
                s => s.BuildProfileEnvironments,
                out SettingsScope scope);

            buildProfileEnvironments.TryFindEnvironmentForKey(profile, out BootstrapEnvironmentAsset environmentAsset);

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