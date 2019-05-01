using System;
using System.Reactive.Disposables;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Droid.Fragments
{
    public abstract class ReactiveFragment<TViewModel> : Fragment
        where TViewModel : class, IViewModel
    {
        protected CompositeDisposable DisposeBag = new CompositeDisposable();

        public TViewModel ViewModel { get; set; }

        protected abstract void InitializeViews(View view);

        protected ReactiveFragment()
        {
        }

        protected ReactiveFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            DisposeBag.Dispose();
            DisposeBag = new CompositeDisposable();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }
    }
}
