using UnityEngine.SceneManagement;

namespace Fabric.Core.Runtime
{
    public class BuildBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        #region Methods

        /// <inheritdoc />
        public void OnPreBootstrap(Bootstrapper bootstrapper) { }

        /// <inheritdoc />
        public void OnPostBootstrap(Bootstrapper bootstrapper)
        {
            SceneManager.LoadScene(1);
        }

        #endregion
    }
}