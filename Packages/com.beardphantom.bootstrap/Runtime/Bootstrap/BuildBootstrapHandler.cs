using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    public class BuildBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        public static readonly BuildBootstrapHandler Instance = new();

        /// <inheritdoc />
        public Awaitable OnPreBootstrapAsync(in BootstrapContext context)
        {
            return AwaitableUtility.GetCompleted();
        }

        /// <inheritdoc />
        public async Awaitable OnPostBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper)
        {
            if (App.IsRunningTests)
            {
                return;
            }

            await SceneManager.LoadSceneAsync(1);
        }
    }
}
