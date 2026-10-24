namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Base interface for all services managed by a <see cref="ServiceLocator" />.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Called to initialize the service during bootstrap.
        /// </summary>
        void InitService(BootstrapContext context);
    }
}