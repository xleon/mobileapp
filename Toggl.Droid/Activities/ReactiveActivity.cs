using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Reactive.Disposables;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Droid.Extensions.Reactive;

namespace Toggl.Droid.Activities
{
    public abstract partial class ReactiveActivity<TViewModel> : AppCompatActivity, IView
        where TViewModel : class, IViewModel
    {
        public CompositeDisposable DisposeBag { get; private set; } = new CompositeDisposable();

        protected abstract void InitializeViews();

        public TViewModel ViewModel { get; private set; }

        protected ReactiveActivity()
        {
        }

        protected ReactiveActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public bool ViewModelWasNotCached()
            => ViewModel == null;

        public void BailOutToSplashScreen()
        {
            StartActivity(new Intent(this, typeof(SplashScreen)).AddFlags(ActivityFlags.TaskOnHome));
            Finish();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ViewModel = AndroidDependencyContainer.Instance
                .ViewModelCache
                .Get<TViewModel>();

            ViewModel?.AttachView(this);
        }

        protected override void OnDestroy()
        {
            ViewModel?.DetachView();
            base.OnDestroy();
            ViewModel?.ViewDestroyed();
        }

        protected override void OnStart()
        {
            base.OnStart();
            ViewModel?.ViewAppearing();
        }

        protected override void OnResume()
        {
            base.OnResume();
            ViewModel?.ViewAppeared();
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModel?.ViewDisappearing();
        }

        protected override void OnStop()
        {
            base.OnStop();
            ViewModel?.ViewDisappeared();
        }

        public override void OnBackPressed()
        {
            if (ViewModel == null)
            {
                base.OnBackPressed();
                return;
            }

            ViewModel.CloseWithDefaultResult();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (requestCode)
            {
                case googleSignInResult:
                    onGoogleSignInResult(data);
                    break;
            }
        }

        protected void SetupToolbar(string title = "", bool showHomeAsUp = true)
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);
            toolbar.Title = title;
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(showHomeAsUp);
            SupportActionBar.SetDisplayShowHomeEnabled(showHomeAsUp);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                ViewModel.CloseWithDefaultResult();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void Close()
        {
            AndroidDependencyContainer.Instance
                .ViewModelCache
                .Clear<TViewModel>();

            Finish();
        }
    }
}
