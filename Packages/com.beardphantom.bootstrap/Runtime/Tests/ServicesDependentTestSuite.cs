#if UNITY_INCLUDE_TESTS
using System.Collections;
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
        /// Waits for the app instance to exist and for bootstrap to reach <see cref="AppBootstrapState.Ready" />,
        /// loading the bootstrap scene additively if needed.
        /// </summary>
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AppInstance appInstance;
            while (!App.TryGetInstance(out appInstance))
            {
                yield return null;
            }

            while (appInstance.BootstrapState != AppBootstrapState.Ready)
            {
                yield return null;
            }
        }
    }
}
#endif