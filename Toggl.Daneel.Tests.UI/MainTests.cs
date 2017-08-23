using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using static Toggl.Daneel.Tests.UI.Extensions.MainExtensions;

namespace Toggl.Daneel.Tests.UI
{
    [TestFixture]
    public class MainTests
    {
        private const string validEmail = "susancalvin@psychohistorian.museum";

        private iOSApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp
                .iOS
                .EnableLocalScreenshots()
                .StartApp();

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
