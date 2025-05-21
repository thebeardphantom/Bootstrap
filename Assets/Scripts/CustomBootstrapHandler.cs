using BeardPhantom.Bootstrap;
using UnityEngine;

public class CustomBootstrapHandler : MonoBehaviour, IPreBootstrapHandler, IPostBootstrapHandler
{
    private IPreBootstrapHandler _defaultPreHandler;

    private IPostBootstrapHandler _defaultPostHandler;

    /// <inheritdoc />
    Awaitable IPreBootstrapHandler.OnPreBootstrapAsync(in BootstrapContext context)
    {
        BootstrapUtility.GetDefaultBootstrapHandlers(out _defaultPreHandler, out _defaultPostHandler);
        Debug.Log("USING CUSTOM OnPreBootstrapAsync");
        return _defaultPreHandler.OnPreBootstrapAsync(context);
    }

    /// <inheritdoc />
    Awaitable IPostBootstrapHandler.OnPostBootstrapAsync(BootstrapContext context, RuntimeBootstrapper runtimeBootstrapper)
    {
        Debug.Log("USING CUSTOM OnPostBootstrapAsync");
        return _defaultPostHandler.OnPostBootstrapAsync(context, runtimeBootstrapper);
    }
}