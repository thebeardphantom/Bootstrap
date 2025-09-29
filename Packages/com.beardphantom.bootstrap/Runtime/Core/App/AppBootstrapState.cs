namespace BeardPhantom.Bootstrap
{
    public enum AppBootstrapState
    {
        None = 0,
        BootstrapHandlerDiscovery = 1,
        PreBootstrap = 2,
        ServiceCreation = 3,
        ServiceDiscovery = 4,
        ServiceBinding = 5,
        ServiceInit = 6,
        AsyncTaskFlush = 7,
        PostBootstrap = 8,
        Ready = 9,
    }
}