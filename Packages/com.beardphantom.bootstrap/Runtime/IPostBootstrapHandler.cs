using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IPostBootstrapHandler
    {
        #region Methods

        UniTask OnPostBootstrap(Bootstrapper bootstrapper);

        #endregion
    }
}