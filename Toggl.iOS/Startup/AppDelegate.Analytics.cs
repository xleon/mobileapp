using System;
using Foundation;
using UIKit;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        private void initializeAnalytics()
        {
            #if USE_ANALYTICS
            Microsoft.AppCenter.AppCenter.Start(
                "{TOGGL_APP_CENTER_ID_IOS}",
                typeof(Microsoft.AppCenter.Crashes.Crashes),
                typeof(Microsoft.AppCenter.Analytics.Analytics));
            Firebase.Core.App.Configure();
            Google.SignIn.SignIn.SharedInstance.ClientID =
                Firebase.Core.App.DefaultInstance.Options.ClientId;
            Adjust.AppDidLaunch(ADJConfig.ConfigWithAppToken("{TOGGL_ADJUST_APP_TOKEN}", AdjustConfig.EnvironmentProduction));
            #endif
        }
    }
}
