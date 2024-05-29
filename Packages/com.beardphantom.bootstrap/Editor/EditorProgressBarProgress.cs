using UnityEditor;

namespace BeardPhantom.Bootstrap.Editor
{
    public class EditorProgressBarProgress : IProgress
    {
        #region Fields

        public static readonly EditorProgressBarProgress Instance = new();

        #endregion

        #region Constructors

        private EditorProgressBarProgress() { }

        #endregion

        #region Methods

        /// <inheritdoc />
        void IProgress.Complete()
        {
            EditorUtility.ClearProgressBar();
        }

        /// <inheritdoc />
        void IProgress.Report(in string info, in float progress)
        {
            EditorUtility.DisplayProgressBar("Edit Mode Bootstrapping", info, progress);
        }

        #endregion
    }
}