using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;
using static Toggl.Shared.Resources;

namespace Toggl.Droid.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme.WhiteStatusBar",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class TokenResetActivity : ReactiveActivity<TokenResetViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TokenResetActivity);
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
            InitializeViews();

            toolbar.Title = LoginTitle;
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetDisplayShowHomeEnabled(false);
            this.CancelAllNotifications();

            emailLabel.Text = ViewModel.Email.ToString();

            passwordEditText
                .Rx().Text()
                .Subscribe(ViewModel.Password)
                .DisposedBy(DisposeBag);

            ViewModel.Done.Executing
                .Subscribe(progressBar.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            signoutLabel.Rx()
                .BindAction(ViewModel.SignOut)
                .DisposedBy(DisposeBag);

            doneButton.Rx()
                .BindAction(ViewModel.Done)
                .DisposedBy(DisposeBag);
        }
    }
}
