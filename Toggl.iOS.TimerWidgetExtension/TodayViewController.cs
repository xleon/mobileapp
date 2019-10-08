using System;
using Foundation;
using NotificationCenter;
using Toggl.iOS.ExtensionKit;
using Toggl.iOS.ExtensionKit.Extensions;
using Toggl.iOS.ExtensionKit.Models;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.TimerWidgetExtension
{
    public partial class TodayViewController : UIViewController, INCWidgetProviding
    {

        private readonly NetworkingHandler networkingHandler
            = new NetworkingHandler(APIHelper.GetTogglAPI());

        private NSTimer elapsedTimeTimer;

        protected TodayViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            StartButton.AddTarget(startTimeEntry, UIControlEvent.TouchUpInside);
            StopButton.AddTarget(stopTimeEntry, UIControlEvent.TouchUpInside);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SharedStorage.Instance.SetWidgetUpdatedDate(DateTimeOffset.Now);

            var timeEntry = SharedStorage.Instance.GetRunningTimeEntryViewModel();
            if (timeEntry == null)
                renderEmptyTimeEntry();
            else
                renderRunningTimeEntry(timeEntry);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            elapsedTimeTimer?.Invalidate();
            elapsedTimeTimer = null;
        }

        private async void startTimeEntry(object sender, EventArgs e)
        {
            var workspaceId = SharedStorage.Instance.GetDefaultWorkspaceId();
            var timeEntry = new TimeEntry(
                workspaceId: workspaceId,
                projectId: null,
                taskId: null,
                billable: false,
                start: DateTimeOffset.Now,
                duration: null,
                description: "",
                tagIds: new long[0],
                userId: (long)SharedStorage.Instance.GetUserId(),
                id: 0,
                serverDeletedAt: null,
                at: DateTimeOffset.Now);
            var timeEntryViewModel = new TimeEntryViewModel(
                timeEntry: timeEntry,
                projectName: null,
                projectColor: null,
                taskName: null,
                clientName: null
            );
            await networkingHandler.StartTimeEntry(timeEntry);
            SharedStorage.Instance.SetRunningTimeEntry(timeEntry);
            renderRunningTimeEntry(timeEntryViewModel);
        }

        private async void stopTimeEntry(object sender, EventArgs e)
        {
            await networkingHandler.StopRunningTimeEntry();

            elapsedTimeTimer?.Invalidate();
            elapsedTimeTimer = null;
            renderEmptyTimeEntry();
        }

        private void renderEmptyTimeEntry()
        {
            DescriptionLabel.Text = Resources.NoDescription;
            ProjectNameLabel.Text = string.Empty;
            ProjectNameLabel.Hidden = DotView.Hidden = true;

            var durationFormat = (DurationFormat)SharedStorage.Instance.GetDurationFormat();
            DurationLabel.Text = TimeSpan.FromSeconds(0).ToFormattedString(durationFormat);

            StartButton.Hidden = false;
            StopButton.Hidden = true;
        }

        private void renderRunningTimeEntry(TimeEntryViewModel timeEntry)
        {
            StartButton.Hidden = true;
            StopButton.Hidden = false;
            DescriptionLabel.Text = timeEntry.Description == string.Empty
                ? Resources.NoDescription
                : timeEntry.Description;
            if (timeEntry.ProjectName != null)
            {
                ProjectNameLabel.Hidden = false;
                ProjectNameLabel.Text = timeEntry.ProjectName;

                DotView.Hidden = false;
                DotView.BackgroundColor = new Color(timeEntry.ProjectColor).ToNativeColor();
            }
            else
            {
                ProjectNameLabel.Hidden = DotView.Hidden = true;
            }


            var durationFormat = (DurationFormat)SharedStorage.Instance.GetDurationFormat();
            updateDurationLabel();

            elapsedTimeTimer?.Invalidate();
            elapsedTimeTimer = NSTimer.CreateRepeatingScheduledTimer(1, (timer) => updateDurationLabel());

            void updateDurationLabel()
            {
                var time = DateTime.Now - timeEntry.StartTime;
                DurationLabel.Text = time.ToFormattedString(durationFormat);
            }
        }

        [Export("widgetPerformUpdateWithCompletionHandler:")]
        public void WidgetPerformUpdate(Action<NCUpdateResult> completionHandler)
        {
            completionHandler(NCUpdateResult.NewData);
        }
    }
}
