using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Default pre/post bootstrap handler used for standalone builds. On post-bootstrap, loads the scene at
    /// build index 1 unless the app is running tests.
    /// </summary>
    [Serializable]
    public class BuildBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        /// <summary>
        /// The shared instance used as the default bootstrap handler for builds.
        /// </summary>
        public static readonly BuildBootstrapHandler Instance = new();

        /// <inheritdoc />
        public Awaitable OnPreBootstrapAsync(in BootstrapContext context)
        {
            return AwaitableUtility.Completed;
        }

        /// <inheritdoc />
        public async Awaitable OnPostBootstrapAsync(BootstrapContext context)
        {
            if (App.Instance.IsRunningTests)
            {
                return;
            }

            await SceneManager.LoadSceneAsync(1);
        }
    }
}