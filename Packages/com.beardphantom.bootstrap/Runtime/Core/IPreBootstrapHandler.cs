using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Implemented by types that need to run logic before an <see cref="AppInstance" /> bootstraps its services.
    /// </summary>
    public interface IPreBootstrapHandler
    {
        /// <summary>
        /// Invoked before service discovery and binding occur during bootstrap.
        /// </summary>
        Awaitable OnPreBootstrapAsync(BootstrapContext context);
    }
}