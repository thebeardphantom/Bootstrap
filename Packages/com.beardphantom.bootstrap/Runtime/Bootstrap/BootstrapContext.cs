namespace BeardPhantom.Bootstrap
{
    public readonly struct BootstrapContext
    {
        #region Fields

        public readonly Bootstrapper Bootstrapper;

        #endregion

        #region Constructors

        public BootstrapContext(Bootstrapper bootstrapper)
        {
            Bootstrapper = bootstrapper;
        }

        #endregion
    }
}