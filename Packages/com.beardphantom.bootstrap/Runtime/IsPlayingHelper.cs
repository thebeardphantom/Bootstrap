#if UNITY_EDITOR
using UnityEditor;

namespace BeardPhantom.Bootstrap
{
    [InitializeOnLoad]
    internal static class IsPlayingHelper
    {
        #region Constructors

        static IsPlayingHelper()
        {
            EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        }

        #endregion

        #region Methods

        private static void OnPlaymodeStateChanged(PlayModeStateChange state)
        {
            App.IsPlaying = state == PlayModeStateChange.EnteredPlayMode;
        }

        #endregion
    }
}
#endif