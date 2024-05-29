namespace BeardPhantom.Bootstrap
{
    public class BootstrapContext
    {
        #region Properties

        public IProgress Progress { get; set; }

        #endregion

        #region Constructors

        public BootstrapContext(IProgress progress)
        {
            Progress = progress;
        }

        #endregion
    }
}