using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public interface ILateInitBootstrapService : IBootstrapService
    {
        /// <summary>
        /// Called when all services have been initalized. At this point all services are considered ready for
        /// "cooperative" work. By this point all services should be considered "ready"
        /// </summary>
        /// <param name="context"></param>
        Awaitable LateInitServiceAsync(BootstrapContext context);
    }
}