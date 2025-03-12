#if UNITY_EDITOR
namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        public static void Deinitialize(AppInitMode mode)
        {
            // Log.Info($"{nameof(DeinitializeIfInMode)} not in mode {mode}.");
            Logging.Info($"{nameof(DeinitializeIfInMode)}({mode})");
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