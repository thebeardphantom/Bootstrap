using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IPostBootstrapHandler
    {
        #region Methods

        UniTask OnPostBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper);

        #endregion
    }
}