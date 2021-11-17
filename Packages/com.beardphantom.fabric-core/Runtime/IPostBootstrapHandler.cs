namespace BeardPhantom.Fabric.Core
{
    public interface IPostBootstrapHandler
    {
        #region Methods

        void OnPostBootstrap(Bootstrapper bootstrapper);

        #endregion
    }
}