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

        private NetworkingHandler networkingHandler;

        private NSTimer elapsedTimeTimer;
        private UITapGestureRecognizer tapGestureRecognizer;

        protected TodayViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            StartButton.AddTarget(startTimeEntry, UIControlEvent.TouchUpInside);
            StopButton.AddTarget(stopTimeEntry, UIControlEvent.TouchUpInside);

            tapGestureRecognizer = new UITapGestureRecognizer(() =>
            {
                ExtensionContext?.OpenUrl(new Uri(ApplicationUrls.Main.Default), null);
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            if (SharedStorage.Instance.GetApiToken() == null)
            {
                renderErrorState(Resources.WidgetLogInToTrackTime);
            }
            else
            {
                networkingHandler = new NetworkingHandler(APIHelper.GetTogglAPI());
                var timeEntry = SharedStorage.Instance.GetRunningTimeEntryViewModel();
                if (timeEntry == null)
                    renderEmptyTimeEntry();
                else
                    renderRunningTimeEntry(timeEntry);
            }

            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SharedStorage.Instance.SetWidgetUpdatedDate(DateTimeOffset.Now);
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
            var createdTimeEntry = await networkingHandler.StartTimeEntry(timeEntry);
            SharedStorage.Instance.SetRunningTimeEntry(createdTimeEntry);
            var timeEntryViewModel = SharedStorage.Instance.GetRunningTimeEntryViewModel();
            renderRunningTimeEntry(timeEntryViewModel);
        }

        private async void stopTimeEntry(object sender, EventArgs e)
        {
            await networkingHandler.StopRunningTimeEntry();
            SharedStorage.Instance.SetRunningTimeEntry(null);

            elapsedTimeTimer?.Invalidate();
            elapsedTimeTimer = null;
            renderEmptyTimeEntry();
        }

        private void renderEmptyTimeEntry()
        {
            View.RemoveGestureRecognizer(tapGestureRecognizer);
            ErrorMessageLabel.Hidden = true;
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
            View.RemoveGestureRecognizer(tapGestureRecognizer);
            ErrorMessageLabel.Hidden = true;
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

        private void renderErrorState(string error)
        {
            ErrorMessageLabel.Hidden = false;
            ErrorMessageLabel.Text = error;

            View.AddGestureRecognizer(tapGestureRecognizer);

            StopButton.Hidden = true;
            StartButton.Hidden = true;
            DurationLabel.Hidden = true;
            DescriptionLabel.Hidden = true;
            ProjectNameLabel.Hidden = true;
            DotView.Hidden = true;

            elapsedTimeTimer?.Invalidate();
            elapsedTimeTimer = null;
        }

        [Export("widgetPerformUpdateWithCompletionHandler:")]
        public void WidgetPerformUpdate(Action<NCUpdateResult> completionHandler)
        {
            completionHandler(NCUpdateResult.NewData);
        }
    }
}
