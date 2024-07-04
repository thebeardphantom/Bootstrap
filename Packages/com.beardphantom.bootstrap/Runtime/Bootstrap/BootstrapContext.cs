namespace BeardPhantom.Bootstrap
{
    public readonly struct BootstrapContext
    {
        public readonly Bootstrapper Bootstrapper;

        public BootstrapContext(Bootstrapper bootstrapper)
        {
            Bootstrapper = bootstrapper;
        }
    }
}