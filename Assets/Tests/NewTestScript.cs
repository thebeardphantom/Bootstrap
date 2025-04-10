using BeardPhantom.Bootstrap;
using BeardPhantom.Bootstrap.Tests;
using NUnit.Framework;

namespace Tests
{
    public class NewTestScript : ServicesDependentTestSuite
    {
        [Test]
        public void AppLocate_DoesNotThrow_ServicesSetup()
        {
            Assert.DoesNotThrow(() => App.Locate<TestService>());
        }
    }
}