using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IPostBootstrapHandler
    {
        #region Methods

        UniTask OnPostBootstrapAsync(Bootstrapper bootstrapper);

        #endregion
    }
}