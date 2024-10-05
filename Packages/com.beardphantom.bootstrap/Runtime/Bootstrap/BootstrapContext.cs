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
}