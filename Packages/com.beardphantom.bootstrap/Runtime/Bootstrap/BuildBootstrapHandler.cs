using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    public class BuildBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        #region Methods

        /// <inheritdoc />
        public UniTask OnPreBootstrapAsync(in BootstrapContext context)
        {
            return default;
        }

        /// <inheritdoc />
        public UniTask OnPostBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper)
        {
            SceneManager.LoadScene(1);
            return UniTask.NextFrame();
        }

        #endregion
    }
}