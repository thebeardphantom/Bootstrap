using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Implemented by types that need to run logic before an <see cref="AppInstance"/> bootstraps its services.
    /// </summary>
    public interface IPreBootstrapHandler
    {
        /// <summary>
        /// Invoked before service discovery and binding occur during bootstrap.
        /// </summary>
        /// <param name="context">The bootstrap context for the app instance being initialized.</param>
        Awaitable OnPreBootstrapAsync(in BootstrapContext context);
    }
}