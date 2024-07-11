using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    public class BuildBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        public static readonly BuildBootstrapHandler Instance = new();

        /// <inheritdoc />
        public UniTask OnPreBootstrapAsync(in BootstrapContext context)
        {
            return default;
        }

        /// <inheritdoc />
        public async UniTask OnPostBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper)
        {
            if (App.IsRunningTests)
            {
                return;
            }

            await SceneManager.LoadSceneAsync(1).ToUniTask();
        }
    }
}