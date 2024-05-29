namespace BeardPhantom.Bootstrap
{
    public interface IProgress
    {
        #region Methods

        void Complete();

        void Report(in string info, in float progress);

        #endregion
    }
}