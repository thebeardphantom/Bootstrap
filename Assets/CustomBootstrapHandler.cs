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
    UniTask IPreBootstrapHandler.OnPreBootstrapAsync(Bootstrapper bootstrapper)
    {
        BootstrapUtility.GetDefaultBoostrapHandlers(out _defaultPreHandler, out _defaultPostHandler);
        Debug.Log("USING CUSTOM OnPreBootstrapAsync");
        return _defaultPreHandler.OnPreBootstrapAsync(bootstrapper);
    }

    /// <inheritdoc />
    UniTask IPostBootstrapHandler.OnPostBootstrapAsync(Bootstrapper bootstrapper)
    {
        Debug.Log("USING CUSTOM OnPostBootstrapAsync");
        return _defaultPostHandler.OnPostBootstrapAsync(bootstrapper);
    }

    #endregion
}