using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IPreBootstrapHandler
    {
        #region Methods

        UniTask OnPreBootstrapAsync(in BootstrapContext context);

        #endregion
    }
}