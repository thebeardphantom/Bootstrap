using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Implemented by types that need to run logic after an <see cref="AppInstance"/> finishes bootstrapping its services.
    /// </summary>
    public interface IPostBootstrapHandler
    {
        /// <summary>
        /// Invoked after service discovery, binding, and initialization have completed during bootstrap.
        /// </summary>
        /// <param name="context">The bootstrap context for the app instance being initialized.</param>
        Awaitable OnPostBootstrapAsync(BootstrapContext context);
    }
}