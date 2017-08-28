using NUnit.Framework;
using Xamarin.UITest.iOS;
using static Toggl.Daneel.Tests.UI.Extensions.MainExtensions;

namespace Toggl.Daneel.Tests.UI
{
    [TestFixture]
    public sealed class MainTests
    {
        private const string validEmail = "susancalvin@psychohistorian.museum";

        private iOSApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = Configuration.GetApp();

            app.WaitForMainScreen();
        }

        [Test]
        public void TappingTheStopButtonStopsTheRunningTimeEntry()
        {
            app.StartTimeEntryWithDescription("Testing the Toggl app");

            app.Tap(Main.StopTimeEntryButton);
            app.WaitForElement(Main.StartTimeEntryButton);
        }
    }
}
