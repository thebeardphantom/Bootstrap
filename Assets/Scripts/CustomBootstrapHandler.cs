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
    async Awaitable IPreBootstrapHandler.OnPreBootstrapAsync(BootstrapContext context)
    {
        if (!App.TryGetInstance<RuntimeAppInstance>(out var runtimeAppInstance))
        {
            return;
        }

        runtimeAppInstance.GetDefaultBootstrapHandlers(out _defaultPreHandler, out _defaultPostHandler);

        Debug.Log("USING CUSTOM OnPreBootstrapAsync");
        await _defaultPreHandler.OnPreBootstrapAsync(context);
    }

    /// <inheritdoc />
    async Awaitable IPostBootstrapHandler.OnPostBootstrapAsync(BootstrapContext context)
    {
        if (_defaultPostHandler == null)
        {
            return;
        }

        Debug.Log("USING CUSTOM OnPostBootstrapAsync");
        await _defaultPostHandler.OnPostBootstrapAsync(context);
    }
}