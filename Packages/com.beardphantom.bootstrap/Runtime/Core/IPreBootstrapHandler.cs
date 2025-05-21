using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public interface IPreBootstrapHandler
    {
        Awaitable OnPreBootstrapAsync(in BootstrapContext context);
    }
}