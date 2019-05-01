using System;
using System.Reactive.Disposables;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Droid.Fragments
{
    public abstract class ReactiveDialogFragment<TViewModel> : DialogFragment
        where TViewModel : class, IViewModel
    {
        protected CompositeDisposable DisposeBag = new CompositeDisposable();

        protected abstract void InitializeViews(View view);

        public TViewModel ViewModel { get; set; }

        protected ReactiveDialogFragment()
        {
        }

        protected ReactiveDialogFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }
    }
}
