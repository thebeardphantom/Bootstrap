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
        ServiceBinding,
        ServiceInit,
        AsyncTaskFlush,
        ServiceActivation,
        PostBootstrap,
        Ready,
    }
}