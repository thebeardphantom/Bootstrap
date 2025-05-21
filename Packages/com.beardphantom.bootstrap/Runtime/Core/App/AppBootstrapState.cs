namespace BeardPhantom.Bootstrap
{
    public enum AppBootstrapState
    {
        None = 0,
        BootstrapHandlerDiscovery = 1,
        PreBootstrap = 2,
        ServicePrefabLoad = 3,
        ServiceCreation = 4,
        ServiceDiscovery = 5,
        ServiceBinding = 6,
        ServiceInit = 7,
        AsyncTaskFlush = 8,
        ServiceActivation = 9,
        PostBootstrap = 10,
        Ready = 11,
    }
}