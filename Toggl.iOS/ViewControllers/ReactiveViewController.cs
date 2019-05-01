using System;
using System.Reactive.Disposables;
using Toggl.Core.UI.ViewModels;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public abstract class ReactiveViewController<TViewModel> : UIViewController
        where TViewModel : IViewModel
    {
        public CompositeDisposable DisposeBag { get; private set; } = new CompositeDisposable();

        public TViewModel ViewModel { get; set; }

        protected ReactiveViewController(string nibName)
            : base(nibName, null) { }

        protected ReactiveViewController(IntPtr handle)
            : base(handle) { }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }
    }

    public abstract class ReactiveTableViewController<TViewModel> : UITableViewController
        where TViewModel : IViewModel
    {
        public CompositeDisposable DisposeBag { get; private set; } = new CompositeDisposable();

        public TViewModel ViewModel { get; set; }

        protected ReactiveTableViewController()
            : base(null, null) { }

        protected ReactiveTableViewController(IntPtr handle)
            : base(handle) { }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }
    }
}
