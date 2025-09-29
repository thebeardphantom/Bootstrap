using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public interface IPostBootstrapHandler
    {
        Awaitable OnPostBootstrapAsync(BootstrapContext context);
    }
}