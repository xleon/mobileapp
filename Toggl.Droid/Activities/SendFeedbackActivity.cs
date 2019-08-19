using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Networking.Exceptions;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.StateVisible,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public partial class SendFeedbackActivity : ReactiveActivity<SendFeedbackViewModel>
    {
        private bool sendEnabled;
        private Subject<Unit> sendFeedbackSubject = new Subject<Unit>();

        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppTheme_Light);
            base.OnCreate(bundle);
            if (ViewModelWasNotCached())
            {
                BailOutToSplashScreen();
                return;
            }
            SetContentView(Resource.Layout.SendFeedbackActivity);
            OverridePendingTransition(Resource.Animation.abc_slide_in_right, Resource.Animation.abc_fade_out);

            InitializeViews();
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = Shared.Resources.SubmitFeedback;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            oopsTextView.Text = Shared.Resources.Oops;
            feedbackHelperTitle.Text = Shared.Resources.FeedbackFieldPlaceholder;
            feedbackEditText.Hint = Shared.Resources.FeedbackHint;

            feedbackEditText.Rx().Text()
                .Subscribe(ViewModel.FeedbackText)
                .DisposedBy(DisposeBag);

            errorCard.Rx()
                .BindAction(ViewModel.DismissError)
                .DisposedBy(DisposeBag);

            var sendButtonEnabled = ViewModel.SendEnabled.CombineLatest(ViewModel.IsLoading,
                (sendIsEnabled, isLoading) => sendIsEnabled && !isLoading);
            sendButtonEnabled
                 .Subscribe(onSendEnabled)
                 .DisposedBy(DisposeBag);

            ViewModel.Error
                .Select(selectErrorMessage)
                .Subscribe(errorInfoText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            sendFeedbackSubject
                .Subscribe(ViewModel.Send.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.Error
                .Select(error => error != null)
                .Subscribe(errorCard.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Subscribe(progressBar.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Invert()
                .Subscribe(feedbackEditText.Rx().Enabled())
                .DisposedBy(DisposeBag);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SendFeedbackMenu, menu);
            return true;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var sendMenuItem = menu.FindItem(Resource.Id.SendMenuItem);
            sendMenuItem.SetEnabled(sendEnabled);
            sendMenuItem.SetTitle(Shared.Resources.Send);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.SendMenuItem:
                    sendFeedbackSubject.OnNext(Unit.Default);
                    return true;

                case Android.Resource.Id.Home:
                    ViewModel.CloseWithDefaultResult();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_slide_out_right);
        }

        private void onSendEnabled(bool enabled)
        {
            sendEnabled = enabled;
            InvalidateOptionsMenu();
        }

        private string selectErrorMessage(Exception exception)
            => exception is OfflineException
                ? Shared.Resources.GenericInternetConnectionErrorMessage
                : Shared.Resources.SomethingWentWrongTryAgain;
    }
}
