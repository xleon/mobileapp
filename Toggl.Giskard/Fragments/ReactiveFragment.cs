using System.Reactive.Disposables;
using Android.OS;
using MvvmCross.Droid.Support.V4;
using MvvmCross.ViewModels;

namespace Toggl.Giskard.Fragments
{
    public class ReactiveFragment<TViewModel> : MvxFragment<TViewModel>
        where TViewModel : class, IMvxViewModel
    {
        protected CompositeDisposable DisposeBag = new CompositeDisposable();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }
    }
}
