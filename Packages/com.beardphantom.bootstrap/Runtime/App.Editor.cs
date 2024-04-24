#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Logging;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        #region Methods

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

        #endregion
    }
}
#endif