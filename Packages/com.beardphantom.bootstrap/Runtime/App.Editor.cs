#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    [InitializeOnLoad]
    public static partial class App
    {
        static App()
        {
            Application.quitting += OnApplicationQuitting;
        }

        public static void Deinitialize(AppInitMode mode)
        {
            Log.Info($"{nameof(DeinitializeIfInMode)}({mode})");
            try
            {
                ServiceLocator?.Dispose();
            }
            finally
            {
                SessionEnvironment = default;
                ServiceLocator = default;
                SessionGuid = default;
                IsPlaying = false;
                IsQuitting = false;
                BootstrapState = AppBootstrapState.None;
                InitMode = AppInitMode.Uninitialized;
                AppBootstrapStateChanged = default;
            }
        }

        public static void DeinitializeIfInMode(AppInitMode mode)
        {
            if (InitMode == mode)
            {
                Deinitialize(mode);
            }
        }

        private static bool TryDetermineSessionEnvironmentInEditor(out RuntimeBootstrapEnvironmentAsset environment)
        {
            if (!BootstrapUtility.TryLoadEditModeState(out EditModeState editModeState))
            {
                environment = default;
                return false;
            }

            environment = editModeState.Environment;
            return editModeState.Environment != null;
        }
    }
}
#endif