using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Toggl.Droid.Services;

namespace Toggl.Droid.Widgets.Services
{
    [Service(Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
    class InstallationStateReportService : JobIntentService
    {
        public const string StateParameterName = nameof(StateParameterName);

        public InstallationStateReportService()
        {
        }

        protected InstallationStateReportService(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public static void EnqueueWork(Context context, Intent work)
        {
            var serviceClass = Java.Lang.Class.FromType(typeof(InstallationStateReportService));
            EnqueueWork(context, serviceClass, JobServicesConstants.TimerWidgetInstallStateReportingJobId, work);
        }

        protected override void OnHandleWork(Intent intent)
        {
            var installationState = intent.GetBooleanExtra(StateParameterName, false);

            var analyticsService = AndroidDependencyContainer.Instance.AnalyticsService;
            analyticsService.TimerWidgetInstallStateChange.Track(installationState);
        }
    }
}
