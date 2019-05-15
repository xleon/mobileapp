using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;

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

        public Task Close()
        {
            AndroidDependencyContainer.Instance
                .ViewModelCache
                .Clear<TViewModel>();
            
            Finish();
            
            return Task.CompletedTask;
        }
    }
}
