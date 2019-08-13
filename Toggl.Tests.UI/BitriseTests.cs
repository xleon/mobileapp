using System;
using NUnit.Framework;
using Toggl.Tests.UI.Extensions;
using Xamarin.UITest;

namespace Toggl.Tests.UI
{
    [TestFixture]
    public class BitriseTests
    {
        private IApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = Configuration.GetApp();
        }

        [Test]
        public void Test1()
        {
            app.WaitForMainScreen();
        }

        [Test]
        public void Test2()
        {
            app.WaitForMainScreen();
        }
    }
}
