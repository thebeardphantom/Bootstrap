using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IEarlyInitBootstrapService : IBootstrapService
    {
        #region Methods

        /// <summary>
        /// Called the earliest in the service initialization lifecycle. Useful for services that want to do something before
        /// any other service has initialized.
        /// </summary>
        /// <param name="context"></param>
        UniTask EarlyInitServiceAsync(BootstrapContext context);

        #endregion
    }
}