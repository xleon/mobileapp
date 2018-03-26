using System;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Views.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.Fragments;
using Toggl.Giskard.Views;
using static Android.Support.V7.Widget.Toolbar;
using TextView = Android.Widget.TextView;

namespace Toggl.Giskard.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme")]
    public sealed class EditTimeEntryActivity : MvxAppCompatActivity<EditTimeEntryViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            this.ChangeStatusBarColor(new Color(ContextCompat.GetColor(this, Resource.Color.blueStatusBarBackground)));
            
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.EditActivity);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            setupToolbar();
        }

        private void setupToolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);

            toolbar.Title = Resources.GetString(Resource.String.Edit);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            var closeDrawable = GetDrawable(Resource.Drawable.close);
            closeDrawable.SetColorFilter(Color.White, PorterDuff.Mode.SrcIn);
            SupportActionBar.SetHomeAsUpIndicator(closeDrawable);

            toolbar.NavigationClick += onNavigateBack;
        }

        private void onNavigateBack(object sender, NavigationClickEventArgs e)
        {
            ViewModel.CloseCommand.Execute();
        }
    }
}
