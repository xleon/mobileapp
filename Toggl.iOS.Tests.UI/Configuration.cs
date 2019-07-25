using System.Diagnostics;
using Xamarin.UITest;
using Xamarin.UITest.iOS;

namespace Toggl.Tests.UI
{
    public static class Configuration
    {
        private static string bootedDeviceId = null;

        public static iOSApp GetApp()
        {
            //reinstallApp(bootedDeviceId);

            var app = ConfigureApp.iOS
                .InstalledApp("com.toggl.daneel.debug")
                .DeviceIdentifier(System.Environment.GetEnvironmentVariable("SIMULATOR_UDID"))
                .StartApp();

            bootedDeviceId = app.Device.DeviceIdentifier;

            return app;
        }

        private static void reinstallApp(string deviceId)
        {
            if (deviceId == null)
            {
                var uninstallAppFromBootedSimulatorCmdLine = "simctl uninstall booted com.toggl.daneel.debug";
                var uninstallAppFromBootedSimulatorProcess = Process.Start("xcrun", uninstallAppFromBootedSimulatorCmdLine);
                uninstallAppFromBootedSimulatorProcess.WaitForExit();
                return;
            }

            var deleteCmdLine = string.Format("simctl uninstall {0} com.toggl.daneel.debug", deviceId);
            var deleteProcess = Process.Start("xcrun", deleteCmdLine);
            deleteProcess.WaitForExit();
        }
    }
}
