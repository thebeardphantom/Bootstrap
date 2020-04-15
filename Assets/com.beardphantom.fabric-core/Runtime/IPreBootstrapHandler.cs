namespace Fabric.Core.Runtime
{
    public interface IPreBootstrapHandler
    {
        #region Methods

        void OnPreBootstrap(Bootstrapper bootstrapper);

        #endregion
    }
}