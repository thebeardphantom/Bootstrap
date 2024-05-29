using Cysharp.Threading.Tasks;

namespace BeardPhantom.Bootstrap
{
    public interface ILateInitBootstrapService : IBootstrapService
    {
        #region Methods

        /// <summary>
        /// Called when all services have been initalized. At this point all services are considered ready for
        /// "cooperative" work. By this point all services should be considered "ready"
        /// </summary>
        /// <param name="context"></param>
        UniTask LateInitServiceAsync(BootstrapContext context);

        #endregion
    }
}