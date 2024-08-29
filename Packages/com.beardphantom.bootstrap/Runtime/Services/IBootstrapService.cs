using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public interface IBootstrapService
    {
        /// <summary>
        /// Called when this service should do "non-cooperative" work. There is no guarantee that any other services
        /// are ready by this point.
        /// </summary>
        /// <param name="context"></param>
        Awaitable InitServiceAsync(BootstrapContext context);
    }
}