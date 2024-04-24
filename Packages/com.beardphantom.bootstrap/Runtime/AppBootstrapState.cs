namespace BeardPhantom.Bootstrap
{
    public enum AppBootstrapState
    {
        None,
        BootstrapHandlerDiscovery,
        PreBootstrap,
        ServiceCreation,
        ServiceDiscovery,
        ServiceEarlyInit,
        ServiceInit,
        ServiceLateInit,
        ServiceActivation,
        PostBoostrap,
        Ready
    }
}