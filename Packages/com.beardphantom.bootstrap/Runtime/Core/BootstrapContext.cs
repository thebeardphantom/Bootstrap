namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Carries state through an <see cref="AppInstance" />'s bootstrap process, passed to
    /// <see cref="IPreBootstrapHandler" /> and <see cref="IPostBootstrapHandler" /> implementations.
    /// </summary>
    public class BootstrapContext
    {
        /// <summary>
        /// The task scheduler associated with the <see cref="AppInstance" /> being bootstrapped.
        /// </summary>
        public readonly TaskScheduler Scheduler;

        /// <summary>
        /// Creates a new bootstrap context.
        /// </summary>
        public BootstrapContext(TaskScheduler scheduler)
        {
            Scheduler = scheduler;
        }
    }
}