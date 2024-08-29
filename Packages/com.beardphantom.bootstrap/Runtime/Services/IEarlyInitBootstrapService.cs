using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public interface IEarlyInitBootstrapService : IBootstrapService
    {
        /// <summary>
        /// Called the earliest in the service initialization lifecycle. Useful for services that want to do something before
        /// any other service has initialized.
        /// </summary>
        /// <param name="context"></param>
        Awaitable EarlyInitServiceAsync(BootstrapContext context);
    }
}