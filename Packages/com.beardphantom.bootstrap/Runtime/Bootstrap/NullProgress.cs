namespace BeardPhantom.Bootstrap
{
    public class NullProgress : IProgress
    {
        #region Fields

        public static readonly NullProgress Instance = new();

        #endregion

        #region Methods

        /// <inheritdoc />
        void IProgress.Complete() { }

        /// <inheritdoc />
        void IProgress.Report(in string info, in float progress) { }

        #endregion
    }
}