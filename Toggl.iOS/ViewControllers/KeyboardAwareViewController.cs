using Foundation;
using Toggl.Core.UI.ViewModels;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public abstract class KeyboardAwareViewController<TViewModel> : ReactiveViewController<TViewModel>
        where TViewModel : IViewModel
    {
        private NSObject willShowNotification;
        private NSObject willHideNotification;

        protected KeyboardAwareViewController(string nibName)
            : base(nibName) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                return;

            willShowNotification = UIKeyboard.Notifications.ObserveWillShow(KeyboardWillShow);
            willHideNotification = UIKeyboard.Notifications.ObserveWillHide(KeyboardWillHide);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                return;

            NSNotificationCenter.DefaultCenter.RemoveObserver(willShowNotification);
            NSNotificationCenter.DefaultCenter.RemoveObserver(willHideNotification);
        }

        protected abstract void KeyboardWillShow(object sender, UIKeyboardEventArgs e);

        protected abstract void KeyboardWillHide(object sender, UIKeyboardEventArgs e);
    }
}
