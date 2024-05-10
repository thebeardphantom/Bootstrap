namespace BeardPhantom.Bootstrap
{
    public enum AppBootstrapState
    {
        None,
        BootstrapHandlerDiscovery,
        PreBootstrap,
        ServicePrefabLoad,
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