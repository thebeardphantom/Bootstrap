// #undef UNITY_EDITOR

namespace BeardPhantom.Bootstrap
{
    public partial class BootstrapContext
    {
        public readonly Bootstrapper Bootstrapper;

        public BootstrapContext(Bootstrapper bootstrapper)
        {
            Bootstrapper = bootstrapper;
        }
    }

#if UNITY_EDITOR
    public partial class BootstrapContext
    {
        public EditModeState EditModeState { get; set; }
    }
#endif
}