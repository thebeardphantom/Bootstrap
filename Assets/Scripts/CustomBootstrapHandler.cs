using BeardPhantom.Bootstrap;
using System;
using UnityEngine;

[Serializable]
public class CustomBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
{
    private IPreBootstrapHandler _defaultPreHandler;

    private IPostBootstrapHandler _defaultPostHandler;

    [field: SerializeField]
    private int IntProperty { get; set; }

    [field: SerializeField]
    private float FloatProperty { get; set; }

    [field: SerializeField]
    private string StringProperty { get; set; }

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