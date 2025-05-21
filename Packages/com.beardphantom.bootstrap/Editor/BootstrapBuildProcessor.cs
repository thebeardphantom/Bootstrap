using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Editor.Environment;
using BeardPhantom.Bootstrap.Editor.Settings;
using BeardPhantom.Bootstrap.Environment;
using System.Collections.Generic;
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

            MappedEnvironmentCollection<BuildProfile> buildProfileEnvironments =
                BootstrapEditorSettingsUtility.GetValue(a => a.BuildProfileEnvironments);
            if (buildProfileEnvironments == null)
            {
                return false;
            }

            foreach (KeyValuePair<BuildProfile, BootstrapEnvironmentAsset> mappedEnvironment in buildProfileEnvironments)
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

            int subtarget = BuildProfileUtility.GetSubtarget(buildSummary);
            NamedBuildTarget namedBuildTarget = BuildProfileUtility.NamedBuildTargetFromTargetAndSubTarget(
                buildSummary.platform,
                subtarget);
            var platformId = BuildProfileUtility.GetGuidFromBuildTarget(namedBuildTarget, buildSummary.platform).ToString();

            MappedEnvironmentCollection<string> platformEnvironments =
                BootstrapEditorSettingsUtility.GetValue(a => a.PlatformEnvironments);
            if (platformEnvironments == null)
            {
                return false;
            }

            foreach (KeyValuePair<string, BootstrapEnvironmentAsset> mappedEnvironment in platformEnvironments)
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
            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            using (ListPool<Object>.Get(out List<Object> list))
            {
                list.AddRange(preloadedAssets);
                list.Add(environment);
                PlayerSettings.SetPreloadedAssets(list.ToArray());
            }
        }

        private static void UnpackEnvironmentAssets()
        {
            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            using (ListPool<Object>.Get(out List<Object> list))
            {
                list.AddRange(preloadedAssets);
                list.RemoveAll(i => i is BootstrapEnvironmentAsset);
                PlayerSettings.SetPreloadedAssets(list.ToArray());
            }
        }

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            if (TryGetEnvironmentForBuildProfile(out BootstrapEnvironmentAsset environment))
            {
                PackEnvironmentAsset(environment);
                return;
            }

            if (TryGetEnvironmentForBuildTarget(report.summary, out environment))
            {
                PackEnvironmentAsset(environment);
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