using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface IBootstrapService
    {
        #region Methods

        /// <summary>
        /// Called when this service should do "non-cooperative" work. There is no guarantee that any other services
        /// are ready by this point.
        /// </summary>
        /// <param name="context"></param>
        UniTask InitServiceAsync(BootstrapContext context);

        #endregion
    }
}