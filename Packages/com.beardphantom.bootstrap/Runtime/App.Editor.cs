#if UNITY_EDITOR
namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        internal static void CleanupEditorOnly()
        {
            Log.Info("App CleanupEditorOnly");
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
                AppBootstrapStateChanged = default;
            }
        }
    }
}
#endif