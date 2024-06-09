using BeardPhantom.Bootstrap;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CustomBootstrapHandler : MonoBehaviour, IPreBootstrapHandler, IPostBootstrapHandler
{
    #region Fields

    private IPreBootstrapHandler _defaultPreHandler;

    private IPostBootstrapHandler _defaultPostHandler;

    #endregion

    #region Methods

    /// <inheritdoc />
    UniTask IPreBootstrapHandler.OnPreBootstrapAsync(in BootstrapContext context)
    {
        BootstrapUtility.GetDefaultBootstrapHandlers(out _defaultPreHandler, out _defaultPostHandler);
        Debug.Log("USING CUSTOM OnPreBootstrapAsync");
        return _defaultPreHandler.OnPreBootstrapAsync(context);
    }

    /// <inheritdoc />
    UniTask IPostBootstrapHandler.OnPostBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper)
    {
        Debug.Log("USING CUSTOM OnPostBootstrapAsync");
        return _defaultPostHandler.OnPostBootstrapAsync(context, bootstrapper);
    }

    #endregion
}