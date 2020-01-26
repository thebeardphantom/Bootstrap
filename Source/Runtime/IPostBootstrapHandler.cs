namespace ASF.Core.Runtime
{
    public interface IPostBootstrapHandler
    {
        #region Methods

        void OnPostBootstrap(Bootstrapper bootstrapper);

        #endregion
    }
}