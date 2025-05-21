#if UNITY_INCLUDE_TESTS
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace BeardPhantom.Bootstrap.Tests
{
    public abstract class ServicesDependentTestSuite
    {
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