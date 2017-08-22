using Xamarin.UITest;
using Xamarin.UITest.iOS;

namespace Toggl.Daneel.Tests.UI
{
    public static class Configuration
    {
        public static iOSApp GetApp()
            => ConfigureApp
                .iOS
                .AppBundle("../iPhoneSimulator/Debug/Toggl.Daneel.app")
                .EnableLocalScreenshots()
                .StartApp();
    }
}
