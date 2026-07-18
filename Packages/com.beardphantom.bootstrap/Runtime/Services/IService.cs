namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Base interface for all services managed by a <see cref="ServiceLocator"/>.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Called to initialize the service during bootstrap.
        /// </summary>
        /// <param name="context">The bootstrap context for the current initialization.</param>
        void InitService(BootstrapContext context);
    }
}