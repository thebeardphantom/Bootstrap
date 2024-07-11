namespace BeardPhantom.Bootstrap
{
    public class BootstrapContext
    {
        public readonly Bootstrapper Bootstrapper;

        public EditModeState EditModeState { get; set; }

        public BootstrapContext(Bootstrapper bootstrapper)
        {
            Bootstrapper = bootstrapper;
        }
    }
}