namespace BeardPhantom.Bootstrap
{
    public enum AppBootstrapState
    {
        None = 0,
        BootstrapHandlerDiscovery = 1,
        PreBootstrap = 2,
        ServiceDiscovery = 3,
        ServiceBinding = 4,
        ServiceInit = 5,
        AsyncTaskFlush = 6,
        PostBootstrap = 7,
        Ready = 8,
    }
}