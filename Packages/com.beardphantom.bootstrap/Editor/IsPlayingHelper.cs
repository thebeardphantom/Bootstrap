using UnityEditor;

namespace BeardPhantom.Bootstrap
{
    internal static class IsPlayingHelper
    {
        #region Methods

        [InitializeOnEnterPlayMode]
        private static void OnPlaymodeStateChanged()
        {
            App.IsPlaying = true;
        }

        #endregion
    }
}