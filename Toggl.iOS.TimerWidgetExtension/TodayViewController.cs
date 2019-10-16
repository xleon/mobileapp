using System;
using CoreGraphics;
using Foundation;
using NotificationCenter;
using Toggl.iOS.ExtensionKit;
using Toggl.iOS.ExtensionKit.Exceptions;
using Toggl.iOS.ExtensionKit.Extensions;
using Toggl.iOS.ExtensionKit.Models;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.TimerWidgetExtension
{
    public partial class TodayViewController : UIViewController, INCWidgetProviding
    {
        private const double compactModeHeight = 112;
        private const double suggestionCellHeight = 60;
        private const double extraLabelsHeight = 51;
        private const int errorMessageLineSpacing = 2;

        private NetworkingHandler networkingHandler;

        private NSTimer elapsedTimeTimer;
        private UITapGestureRecognizer tapGestureRecognizer;
        private SuggestionsDataSource dataSource;

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

            dataSource = new SuggestionsDataSource();
            SuggestionsTableView.DataSource = dataSource;
        }

        public override void ViewWillAppear(bool animated)
        {

            ExtensionContext.SetWidgetLargestAvailableDisplayMode(NCWidgetDisplayMode.Expanded);
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

        [Export("widgetActiveDisplayModeDidChange:withMaximumSize:")]
        public void WidgetActiveDisplayModeDidChange(NCWidgetDisplayMode activeDisplayMode, CGSize maxSize)
        {
            var suggestionsCount = 3;
            PreferredContentSize = activeDisplayMode == NCWidgetDisplayMode.Compact
                ? maxSize
                : new CGSize(maxSize.Width, compactModeHeight + suggestionsCount * suggestionCellHeight + extraLabelsHeight);
            setConstraintsForDisplayMode(activeDisplayMode, suggestionsCount);
        }

        private void setConstraintsForDisplayMode(NCWidgetDisplayMode activeDisplayMode, int suggestionsCount)
        {
            switch (activeDisplayMode)
            {
                case NCWidgetDisplayMode.Compact:
                    RunningTimerContainerCompactBottomConstraint.Active = true;
                    SuggestionsContainerExpandedBottomConstraint.Active = false;
                    break;
                case NCWidgetDisplayMode.Expanded:
                    RunningTimerContainerCompactBottomConstraint.Active = false;
                    SuggestionsContainerExpandedBottomConstraint.Active = true;
                    break;
            }
            SuggestionsTableViewHeightConstraint.Constant = 60 * suggestionsCount;
        }

        private async void startTimeEntry(object sender, EventArgs e)
        {
            try
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
            catch (ApiException)
            {
                renderErrorState(Resources.WidgetApiError);
            }
            catch
            {
                renderErrorState(Resources.WidgetGenericError);
            }
        }

        private async void stopTimeEntry(object sender, EventArgs e)
        {
            try
            {
                await networkingHandler.StopRunningTimeEntry();
                SharedStorage.Instance.SetRunningTimeEntry(null);

                elapsedTimeTimer?.Invalidate();
                elapsedTimeTimer = null;
                renderEmptyTimeEntry();
            }
            catch (ApiException)
            {
                renderErrorState(Resources.WidgetApiError);
            }
            catch (NoRunningEntryException)
            {
                renderErrorState(Resources.WidgetNoRunningTimeEntryError);
            }
            catch
            {
                renderErrorState(Resources.WidgetGenericError);
            }
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
            ExtensionContext.SetWidgetLargestAvailableDisplayMode(NCWidgetDisplayMode.Compact);
            ErrorMessageLabel.Hidden = false;
            ErrorMessageLabel.Text = error;
            ErrorMessageLabel.SetLineSpacing(errorMessageLineSpacing, UITextAlignment.Center);

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
