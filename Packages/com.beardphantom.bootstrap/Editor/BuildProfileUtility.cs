using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;

namespace BeardPhantom.Bootstrap.Editor
{
    /// <summary>
    /// Wraps internal UnityEditor build profile APIs via reflection to expose functionality not otherwise public.
    /// </summary>
    [InitializeOnLoad]
    public static class BuildProfileUtility
    {
        /// <summary>
        /// Forwards add/remove subscriptions to Unity's internal <c>BuildPlayerWindow.drawingMultiplayerBuildOptions</c> event.
        /// </summary>
        public static event Action<BuildProfile> DrawMultiplayerBuildOptions
        {
            add
            {
                try
                {
                    s_oneArgArray[0] = value;
                    s_drawMultiplayerBuildOptionsEvt.GetAddMethod(true).Invoke(null, s_oneArgArray);
                }
                finally
                {
                    s_oneArgArray[0] = null;
                }
            }
            remove
            {
                try
                {
                    s_oneArgArray[0] = value;
                    s_drawMultiplayerBuildOptionsEvt.GetRemoveMethod(true).Invoke(null, s_oneArgArray);
                }
                finally
                {
                    s_oneArgArray[0] = null;
                }
            }
        }

        private static readonly PropertyInfo s_subTargetProperty;

        private static readonly MethodInfo s_fromTargetAndSubTargetMethod;

        private static readonly MethodInfo s_getGUIDFromBuildTargetMethod;

        private static readonly MethodInfo s_isClassicPlatformProfileMethod;

        private static readonly object[] s_oneArgArray = new object[1];

        private static readonly object[] s_twoArgArray = new object[2];

        private static readonly EventInfo s_drawMultiplayerBuildOptionsEvt;

        static BuildProfileUtility()
        {
            // Get event for drawing in build profile window
            s_drawMultiplayerBuildOptionsEvt = typeof(BuildPlayerWindow)
                .GetEvent("drawingMultiplayerBuildOptions", BindingFlags.Static | BindingFlags.NonPublic);

            // Get function for determining if build profile is a "classic" profile
            Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;
            Type buildProfileContextType = editorAssembly.GetType("UnityEditor.Build.Profile.BuildProfileContext");
            s_isClassicPlatformProfileMethod = buildProfileContextType.GetMethod(
                "IsClassicPlatformProfile",
                BindingFlags.NonPublic | BindingFlags.Static);

            Type buildTargetDiscoveryType = editorAssembly.GetType("UnityEditor.BuildTargetDiscovery");
            MethodInfo[] methods = buildTargetDiscoveryType.GetMethods(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            s_getGUIDFromBuildTargetMethod = methods
                .Where(n => n.Name == "GetGUIDFromBuildTarget")
                .Single(m => m.GetParameters().Length == 2);

            s_fromTargetAndSubTargetMethod = typeof(NamedBuildTarget).GetMethod(
                "FromTargetAndSubtarget",
                BindingFlags.NonPublic | BindingFlags.Static);

            s_subTargetProperty = typeof(BuildSummary).GetProperty("subtarget", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Resolves a <see cref="NamedBuildTarget"/> from a <see cref="BuildTarget"/> and subtarget index via Unity's internal API.
        /// </summary>
        /// <param name="buildTarget">The build target.</param>
        /// <param name="subTarget">The subtarget index.</param>
        public static NamedBuildTarget NamedBuildTargetFromTargetAndSubTarget(BuildTarget buildTarget, int subTarget)
        {
            try
            {
                s_twoArgArray[0] = buildTarget;
                s_twoArgArray[1] = subTarget;
                return (NamedBuildTarget)s_fromTargetAndSubTargetMethod.Invoke(null, s_twoArgArray);
            }
            finally
            {
                s_twoArgArray[0] = null;
                s_twoArgArray[1] = null;
            }
        }

        /// <summary>
        /// Checks whether <paramref name="buildProfile"/> is a "classic" platform profile via Unity's internal API.
        /// </summary>
        /// <param name="buildProfile">The build profile to check.</param>
        /// <returns>True if the profile is a classic platform profile.</returns>
        public static bool IsClassicProfile(BuildProfile buildProfile)
        {
            try
            {
                s_oneArgArray[0] = buildProfile;
                return (bool)s_isClassicPlatformProfileMethod.Invoke(null, s_oneArgArray);
            }
            finally
            {
                s_oneArgArray[0] = null;
            }
        }

        /// <summary>
        /// Resolves the platform GUID for a given <see cref="NamedBuildTarget"/> and <see cref="BuildTarget"/> via Unity's internal API.
        /// </summary>
        /// <param name="namedBuildTarget">The named build target.</param>
        /// <param name="buildTarget">The build target.</param>
        public static UnityEngine.GUID GetGuidFromBuildTarget(NamedBuildTarget namedBuildTarget, BuildTarget buildTarget)
        {
            try
            {
                s_twoArgArray[0] = namedBuildTarget;
                s_twoArgArray[1] = buildTarget;
                return (UnityEngine.GUID)s_getGUIDFromBuildTargetMethod.Invoke(null, s_twoArgArray);
            }
            finally
            {
                s_twoArgArray[0] = null;
                s_twoArgArray[1] = null;
            }
        }

        /// <summary>
        /// Reads the internal subtarget value from a <see cref="BuildSummary"/> via reflection.
        /// </summary>
        /// <param name="reportSummary">The build summary to read from.</param>
        public static int GetSubtarget(BuildSummary reportSummary)
        {
            return (int)s_subTargetProperty.GetGetMethod(true).Invoke(reportSummary, Array.Empty<object>());
        }

        /// <summary>
        /// Reads the serialized platform ID for a <see cref="BuildProfile"/>.
        /// </summary>
        /// <param name="profile">The build profile to read from.</param>
        public static string GetPlatformIdForProfile(BuildProfile profile)
        {
            using var serializedObject = new SerializedObject(profile);
            SerializedProperty platformIdProperty = serializedObject.FindProperty("m_PlatformId");
            return platformIdProperty.stringValue;
        }
    }
}