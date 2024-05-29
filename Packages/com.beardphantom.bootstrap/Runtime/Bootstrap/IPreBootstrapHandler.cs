using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IPreBootstrapHandler
    {
        #region Methods

        UniTask OnPreBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper);

        #endregion
    }
}