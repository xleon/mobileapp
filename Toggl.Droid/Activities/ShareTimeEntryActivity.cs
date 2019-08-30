using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using System;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Transformations;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;
using TagsAdapter = Toggl.Droid.Adapters.SimpleAdapter<string>;
using TextResources = Toggl.Shared.Resources;
using Toggl.Droid.Presentation;
using TimeEntryExtensions = Toggl.Droid.Extensions.TimeEntryExtensions;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class ShareTimeEntryActivity : ReactiveActivity<ShareTimeEntryViewModel>
    {
        public ShareTimeEntryActivity() : base(
            Resource.Layout.ShareTimeEntryActivity,
            Resource.Style.AppTheme_Light_WhiteBackground,
            Transitions.SlideInFromBottom)
        { }

        public ShareTimeEntryActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_slide_out_bottom);
        }

        protected override void InitializeBindings()
        {
            ViewModel.Description
                .Subscribe(descriptionTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.ProjectClient
                .Select(ptc => ptc != null)
                .Subscribe(projectGroup.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.ProjectClient
                .Where(ptc => ptc != null)
                .Subscribe(projectTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Tags
                .Select(tags => tags != null)
                .Subscribe(tagsGroup.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.Tags
                .Where(ptc => ptc != null)
                .Subscribe(tagsTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StartTime
                .Select(time => time.ToString("HH:mm"))
                .Subscribe(startTimeTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StartTime
                .Select(time => time.ToString("yyyy-MM-dd"))
                .Subscribe(startDateTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StopTime
                .Select(time => time.HasValue)
                .Subscribe(stopTimeGroup.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.StopTime
                .Where(time => time.HasValue)
                .Select(time => time.Value.ToString("yyyy-MM-dd"))
                .Subscribe(stopDateTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StopTime
                .Where(time => time.HasValue)
                .Select(time => time.Value.ToString("HH:mm"))
                .Subscribe(stopTimeTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsBillable
                .Subscribe(billableGroup.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.CreateProjectElementsLabel
                .Select(label => label != null)
                .Subscribe(missingProjectLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.CreateProjectElementsLabel
                .Where(label => label != null)
                .Subscribe(missingProjectLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.CreateTagsLabel
                .Select(label => label != null)
                .Subscribe(missingTagsLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.CreateTagsLabel
                .Where(label => label != null)
                .Subscribe(missingTagsLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.ShouldCreateProjectElements
                .Subscribe(projectSwitch.Rx().CheckedObserver())
                .DisposedBy(DisposeBag);

            ViewModel.ShouldCreateTags
                .Subscribe(tagsSwitch.Rx().CheckedObserver())
                .DisposedBy(DisposeBag);

            projectSwitch.Rx().Tap()
                .Subscribe(ViewModel.ToggleProjectElementsCreation.Inputs)
                .DisposedBy(DisposeBag);

            tagsSwitch.Rx().Tap()
                .Subscribe(ViewModel.ToggleTagsCreation.Inputs)
                .DisposedBy(DisposeBag);

            closeButton.Rx().Tap()
                .Subscribe(_ => ViewModel.Close())
                .DisposedBy(DisposeBag);

            acceptButton.Rx().Tap()
                .Subscribe(ViewModel.Save.Inputs)
                .DisposedBy(DisposeBag);
        }

        private void updateUI(SharePayload payload)
        {
            // should not happen here?!?

            descriptionTextView.Text = payload.Description;

            projectGroup.Visibility = payload.ProjectId.HasValue.ToVisibility();
            if (payload.ProjectId.HasValue)
            {
                var task = payload.TaskId.HasValue
                    ? $" : {payload.TaskName}"
                    : "";

                var client = payload.ClientId.HasValue
                    ? $" ({payload.ClientName})"
                    : "";

                projectTextView.Text = $"{payload.ProjectName}{task}{client}";
            }

            tagsGroup.Visibility = (payload.Tags.Length > 0).ToVisibility();
            if (payload.Tags.Length > 0)
                tagsTextView.Text = string.Join(", ", payload.Tags.Select(tag => tag.Name));

            startTimeTextView.Text = payload.Start.ToString("HH:mm");
            startDateTextView.Text = payload.Start.ToString("yyyy-MM-dd");

            stopTimeGroup.Visibility = payload.Stop.HasValue.ToVisibility();
            if (payload.Stop.HasValue)
            {
                stopTimeTextView.Text = payload.Stop.Value.ToString("HH:mm");
                stopDateTextView.Text = payload.Stop.Value.ToString("yyyy-MM-dd");
            }

            billableGroup.Visibility = payload.IsBillable.ToVisibility();
        }
    }
}
