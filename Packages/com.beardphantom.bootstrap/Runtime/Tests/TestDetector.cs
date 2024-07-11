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
            Log.Info("Playmode test execution start detected.");
            App.IsRunningTests = true;
        }

        void ITestRunCallback.RunFinished(ITestResult testResults)
        {
            Log.Info("Playmode test execution finish detected.");
            App.IsRunningTests = false;
        }

        void ITestRunCallback.TestStarted(ITest test) { }

        void ITestRunCallback.TestFinished(ITestResult result) { }
    }
}