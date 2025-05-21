// #undef UNITY_EDITOR

#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
#endif

namespace BeardPhantom.Bootstrap
{
    public partial class BootstrapContext
    {
        public readonly RuntimeBootstrapper RuntimeBootstrapper;

        public readonly TaskScheduler Scheduler;

        public BootstrapContext(RuntimeBootstrapper runtimeBootstrapper, TaskScheduler scheduler)
        {
            RuntimeBootstrapper = runtimeBootstrapper;
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