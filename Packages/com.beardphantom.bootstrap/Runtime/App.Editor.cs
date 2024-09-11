#if UNITY_EDITOR
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
    }
}
#endif