using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    public class BuildBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        #region Methods

        /// <inheritdoc />
        public UniTask OnPreBootstrapAsync(Bootstrapper bootstrapper)
        {
            return default;
        }

        /// <inheritdoc />
        public UniTask OnPostBootstrap(Bootstrapper bootstrapper)
        {
            SceneManager.LoadScene(1);
            return UniTask.NextFrame();
        }

        #endregion
    }
}