namespace BeardPhantom.Fabric.Core
{
    public interface IPreBootstrapHandler
    {
        #region Methods

        void OnPreBootstrap(Bootstrapper bootstrapper);

        #endregion
    }
}