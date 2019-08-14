using System.Diagnostics;
using System;
using System.IO;
using Xamarin.UITest;
using Xamarin.UITest.iOS;

namespace Toggl.Tests.UI
{
    public static class Configuration
    {
        private static string deviceID = Environment.GetEnvironmentVariable("SIMULATOR_UDID");

        public static iOSApp GetApp()
        {
            uninstallApp();

            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent;
            var appBundle = new DirectoryInfo(directoryInfo.FullName + "/iPhoneSimulator/Debug/Toggl.iOS.app");

            var app = ConfigureApp.iOS
                .AppBundle(appBundle.FullName)
                .DeviceIdentifier(deviceID)
                .StartApp();

            return app;
        }

        private static void uninstallApp()
            => Process.Start("xcrun", $"simctl uninstall {deviceID} com.toggl.daneel.debug")
                .WaitForExit();
    }
}
