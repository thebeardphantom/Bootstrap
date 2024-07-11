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
            if (App.BootstrapState == AppBootstrapState.Ready)
            {
                yield break;
            }

            yield return SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
            while (App.BootstrapState != AppBootstrapState.Ready)
            {
                yield return null;
            }
        }
    }
}