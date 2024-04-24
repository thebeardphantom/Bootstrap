using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap.Services
{
    public interface IBootstrapService
    {
        #region Methods

        /// <summary>
        /// Called when this service should do "non-cooperative" work. There is no guarantee that any other services
        /// are ready by this point.
        /// </summary>
        UniTask InitServiceAsync();

        #endregion
    }
}