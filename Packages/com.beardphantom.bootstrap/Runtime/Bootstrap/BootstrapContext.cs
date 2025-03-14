// #undef UNITY_EDITOR

namespace BeardPhantom.Bootstrap
{
    public partial class BootstrapContext
    {
        public readonly Bootstrapper Bootstrapper;

        public readonly AsyncTaskScheduler Scheduler;

        public BootstrapContext(Bootstrapper bootstrapper, AsyncTaskScheduler scheduler)
        {
            Bootstrapper = bootstrapper;
            Scheduler = scheduler;
        }
    }

#if UNITY_EDITOR
    public partial class BootstrapContext
    {
        public EditModeState EditModeState { get; set; }
    }
#endif
}
