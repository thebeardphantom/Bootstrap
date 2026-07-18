#if UNITY_INCLUDE_TESTS
using BeardPhantom.Bootstrap.Tests;
using NUnit.Framework.Interfaces;
using UnityEngine.TestRunner;

[assembly: TestRunCallback(typeof(TestDetector))]

namespace BeardPhantom.Bootstrap.Tests
{
    /// <summary>
    /// Tracks whether Unity's playmode test runner is currently executing tests, exposing this via
    /// <see cref="AppInstance.IsRunningTests"/>.
    /// </summary>
    public class TestDetector : ITestRunCallback
    {
        void ITestRunCallback.RunStarted(ITest testsToRun)
        {
            if (!App.TryGetInstance(out AppInstance appInstance) || appInstance.IsRunningTests)
            {
                return;
            }

            Logging.Info("Playmode test execution start detected.");
            appInstance.IsRunningTests = true;
        }

        void ITestRunCallback.RunFinished(ITestResult testResults)
        {
            if (!App.TryGetInstance(out AppInstance appInstance) || !appInstance.IsRunningTests)
            {
                return;
            }

            Logging.Info("Playmode test execution finish detected.");
            appInstance.IsRunningTests = false;
        }

        void ITestRunCallback.TestStarted(ITest test) { }

        void ITestRunCallback.TestFinished(ITestResult result) { }
    }
}
#endif