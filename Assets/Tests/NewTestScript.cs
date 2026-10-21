using BeardPhantom.Bootstrap;
using BeardPhantom.Bootstrap.Tests;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript : ServicesDependentTestSuite
    {
        [UnityTest]
        public IEnumerator AppLocate_DoesNotThrow_ServicesSetup()
        {
            Debug.Log($"Test is running, waiting 3 seconds.");
            yield return new WaitForSeconds(3f);
            Debug.Log($"Done, doing assert.");
            Assert.DoesNotThrow(() => App.Locate<TestService>());
        }
    }
}