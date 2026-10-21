using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Pool;

namespace BeardPhantom.Bootstrap.Editor
{
    internal class BootstrapBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder { get; }

        private static bool TryGetEnvironmentForBuildProfile(out BootstrapEnvironmentAsset environment)
        {
            environment = null;

            var buildProfile = BuildProfile.GetActiveBuildProfile();
            if (!buildProfile)
            {
                return false;
            }

            var buildProfileEnvironments =
                BootstrapEditorSettingsUtility.GetValue(a => a.BuildProfileEnvironments);
            if (buildProfileEnvironments.IsNull())
            {
                return false;
            }

            foreach (var mappedEnvironment in buildProfileEnvironments)
            {
                if (mappedEnvironment.Key == buildProfile)
                {
                    Logging.Debug($"Using build environment for build profile {buildProfile.name}.");
                    environment = mappedEnvironment.Value;
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetEnvironmentForBuildTarget(BuildSummary buildSummary, out BootstrapEnvironmentAsset environment)
        {
            environment = null;

            var subtarget = BuildProfileUtility.GetSubtarget(buildSummary);
            var namedBuildTarget = BuildProfileUtility.NamedBuildTargetFromTargetAndSubTarget(
                buildSummary.platform,
                subtarget);
            var platformId = BuildProfileUtility.GetGuidFromBuildTarget(namedBuildTarget, buildSummary.platform).ToString();

            var platformEnvironments =
                BootstrapEditorSettingsUtility.GetValue(a => a.PlatformEnvironments);
            if (platformEnvironments.IsNull())
            {
                return false;
            }

            foreach (var mappedEnvironment in platformEnvironments)
            {
                if (mappedEnvironment.Key == platformId)
                {
                    Logging.Debug($"Using build environment for build target {platformId}.");
                    environment = mappedEnvironment.Value;
                    return true;
                }
            }

            return false;
        }

        private static void PackEnvironmentAsset(BootstrapEnvironmentAsset environment)
        {
            Logging.Debug($"Packing environment {environment.name}.");
            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            using (ListPool<Object>.Get(out var list))
            {
                list.AddRange(preloadedAssets);
                list.Add(environment);
                PlayerSettings.SetPreloadedAssets(list.ToArray());
            }
        }

        private static void UnpackEnvironmentAssets()
        {
            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            using (ListPool<Object>.Get(out var list))
            {
                list.AddRange(preloadedAssets);
                list.RemoveAll(i => i is BootstrapEnvironmentAsset);
                PlayerSettings.SetPreloadedAssets(list.ToArray());
            }
        }

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            if (TryGetEnvironmentForBuildProfile(out var environment))
            {
                PackEnvironmentAsset(environment);
                return;
            }

            if (TryGetEnvironmentForBuildTarget(report.summary, out environment))
            {
                PackEnvironmentAsset(environment);
                return;
            }

            var defaultBuildEnvironment =
                BootstrapEditorSettingsUtility.GetValue(t => t.DefaultBuildEnvironment);
            if (defaultBuildEnvironment.IsNotNull())
            {
                PackEnvironmentAsset(defaultBuildEnvironment);
                return;
            }

            Logging.Warn("Unable to determine suitable environment for build.");
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            UnpackEnvironmentAssets();
        }
    }
}