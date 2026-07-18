#if UNITY_INCLUDE_TESTS
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace BeardPhantom.Bootstrap.Tests
{
    /// <summary>
    /// Base class for test suites that require the app to have fully bootstrapped before running, loading the
    /// bootstrap scene if it isn't already active.
    /// </summary>
    public abstract class ServicesDependentTestSuite
    {
        /// <summary>
        /// Waits for the app instance to exist and for bootstrap to reach <see cref="AppBootstrapState.Ready"/>,
        /// loading the bootstrap scene additively if needed.
        /// </summary>
        /// <returns>An enumerator suitable for use as a Unity test coroutine.</returns>
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AppInstance appInstance;
            while (!App.TryGetInstance(out appInstance))
            {
                yield return null;
            }

            if (appInstance.BootstrapState == AppBootstrapState.Ready)
            {
                yield break;
            }

            yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
            while (appInstance.BootstrapState != AppBootstrapState.Ready)
            {
                yield return null;
            }
        }
    }
}
#endif