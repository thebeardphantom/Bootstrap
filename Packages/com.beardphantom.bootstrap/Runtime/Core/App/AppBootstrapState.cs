namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// The phases an <see cref="AppInstance"/> passes through during bootstrap, exposed via
    /// <see cref="AppInstance.BootstrapState"/>.
    /// </summary>
    public enum AppBootstrapState
    {
        /// <summary>
        /// The initial state before bootstrapping has begun.
        /// </summary>
        None = 0,

        /// <summary>
        /// Selecting the pre/post bootstrap handlers to use for this session.
        /// </summary>
        BootstrapHandlerDiscovery = 1,

        /// <summary>
        /// Running the selected <see cref="IPreBootstrapHandler"/>.
        /// </summary>
        PreBootstrap = 2,

        /// <summary>
        /// Discovering the services contained in the session's <see cref="ServiceList"/>.
        /// </summary>
        ServiceDiscovery = 3,

        /// <summary>
        /// Binding discovered services to their lookup types.
        /// </summary>
        ServiceBinding = 4,

        /// <summary>
        /// Initializing bound services in priority order.
        /// </summary>
        ServiceInit = 5,

        /// <summary>
        /// Flushing the <see cref="TaskScheduler"/> queue until it is idle.
        /// </summary>
        AsyncTaskFlush = 6,

        /// <summary>
        /// Running the selected <see cref="IPostBootstrapHandler"/>.
        /// </summary>
        PostBootstrap = 7,

        /// <summary>
        /// Bootstrapping has completed and the app instance is fully running.
        /// </summary>
        Ready = 8,

        /// <summary>
        /// The app instance is quitting and disposing.
        /// </summary>
        Quitting = 9,

        /// <summary>
        /// The app instance is being disposed as part of a reset, before reinitializing.
        /// </summary>
        Resetting = 10,
    }
}