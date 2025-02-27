namespace BeardPhantom.Bootstrap
{
    public class BootstrapContext
    {
        public readonly Bootstrapper Bootstrapper;

        public readonly AsyncTaskScheduler Scheduler;

        public EditModeState EditModeState { get; set; }

        public BootstrapContext(Bootstrapper bootstrapper, AsyncTaskScheduler scheduler)
        {
            Bootstrapper = bootstrapper;
            Scheduler = scheduler;
        }
    }
}