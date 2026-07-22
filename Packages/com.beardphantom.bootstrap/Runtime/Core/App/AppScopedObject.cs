using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Automatically registers this GameObject as an <see cref="App" />-scoped object via
    /// <see cref="AppInstance.ScopeToApp{T}" />.
    /// </summary>
    [DisallowMultipleComponent]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class AppScopedObject : MonoBehaviour
    {
        protected virtual void Awake()
        {
            if (!App.TryGetInstance(out AppInstance appInstance))
            {
                Logging.Warn("No current AppInstance.", this);
                return;
            }

            appInstance.ScopeToApp(gameObject);
        }
    }
}