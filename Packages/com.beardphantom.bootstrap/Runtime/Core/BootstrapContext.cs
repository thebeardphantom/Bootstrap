// #undef UNITY_EDITOR

#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
#endif

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Carries state through an <see cref="AppInstance"/>'s bootstrap process, passed to
    /// <see cref="IPreBootstrapHandler"/> and <see cref="IPostBootstrapHandler"/> implementations.
    /// </summary>
    public partial class BootstrapContext
    {
        /// <summary>
        /// The task scheduler associated with the <see cref="AppInstance"/> being bootstrapped.
        /// </summary>
        public readonly TaskScheduler Scheduler;

        /// <summary>
        /// Creates a new bootstrap context.
        /// </summary>
        /// <param name="scheduler">The task scheduler to associate with the app instance being bootstrapped.</param>
        public BootstrapContext(TaskScheduler scheduler)
        {
            Scheduler = scheduler;
        }
    }

#if UNITY_EDITOR
    public partial class BootstrapContext
    {
        /// <summary>
        /// The editor's current edit mode state, available while bootstrapping outside of play mode.
        /// </summary>
        public EditModeState EditModeState { get; set; }
    }
#endif
}