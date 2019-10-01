using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Toggl.Core;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using static Toggl.Droid.Services.JobServicesConstants;

namespace Toggl.Droid.Services
{
    [Service(
        Exported = true,
        Name = "com.toggl.giskard.TimerBackgroundService",
        Permission = "android.permission.BIND_JOB_SERVICE"
    )]
    public class TimerBackgroundService : JobIntentService
    {
        public const string StartTimeEntryAction = nameof(StartTimeEntryAction);
        public const string StopRunningTimeEntryAction = nameof(StopRunningTimeEntryAction);

        private IInteractorFactory interactorFactory => AndroidDependencyContainer.Instance.InteractorFactory;
        private ITimeService timeService => AndroidDependencyContainer.Instance.TimeService;

        public static void EnqueueStartTimeEntry(Context context, Intent intent)
        {
            var serviceClass = Java.Lang.Class.FromType(typeof(TimerBackgroundService));
            EnqueueWork(context, serviceClass, TimerWidgetStartTimeEntryJobId, intent);
        }

        public static void EnqueueStopTimeEntry(Context context, Intent intent)
        {
            var serviceClass = Java.Lang.Class.FromType(typeof(TimerBackgroundService));
            EnqueueWork(context, serviceClass, TimerWidgetStopRunningTimeEntryJobId, intent);
        }

        protected override void OnHandleWork(Intent intent)
        {
            var action = intent.Action;

            AndroidDependencyContainer.EnsureInitialized(Application.Context);

            switch (action)
            {
                case StartTimeEntryAction:
                    _ = handleStartTimeEntry();
                    break;
                case StopRunningTimeEntryAction:
                    _ = handleStopRunningTimeEntry();
                    break;
                default:
                    throw new InvalidOperationException($"Cannot handle intent with action {action}");
            }
        }

        private async Task handleStartTimeEntry()
        {
            var now = timeService.CurrentDateTime;
            var workspaceId = (await interactorFactory.GetDefaultWorkspace().Execute()).Id;
            var prototype = "".AsTimeEntryPrototype(now, workspaceId);
            await interactorFactory.CreateTimeEntry(prototype, TimeEntryStartOrigin.Widget).Execute();
        }

        private async Task handleStopRunningTimeEntry()
        {
            var now = timeService.CurrentDateTime;
            await interactorFactory.StopTimeEntry(now, TimeEntryStopOrigin.Widget).Execute();
        }
    }
}
