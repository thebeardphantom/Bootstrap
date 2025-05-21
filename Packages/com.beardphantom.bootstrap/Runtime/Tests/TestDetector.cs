#if UNITY_INCLUDE_TESTS
using BeardPhantom.Bootstrap.Tests;
using NUnit.Framework.Interfaces;
using UnityEngine.TestRunner;

[assembly: TestRunCallback(typeof(TestDetector))]

namespace BeardPhantom.Bootstrap.Tests
{
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