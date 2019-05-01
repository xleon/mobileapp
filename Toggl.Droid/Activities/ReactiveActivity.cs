using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using static Toggl.Droid.Services.PermissionsServiceAndroid;

namespace Toggl.Droid.Activities
{
    public abstract partial class ReactiveActivity<TViewModel> : AppCompatActivity, IPermissionAskingActivity, IView
        where TViewModel : class, IViewModel
    {
        public CompositeDisposable DisposeBag { get; private set; } = new CompositeDisposable();

        protected abstract void InitializeViews();

        public TViewModel ViewModel { get; set; }

        public Action<int, string[], Permission[]> OnPermissionChangedCallback { get; set; }

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

        public void MvxInternalStartActivityForResult(Intent intent, int requestCode)
        {
            StartActivityForResult(intent, requestCode);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }

        public Task Close()
        {
            Finish();
            return Task.CompletedTask;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            OnPermissionChangedCallback?.Invoke(requestCode, permissions, grantResults);
            OnPermissionChangedCallback = null;
        }
    }
}
