using System.Threading.Tasks;
using Toggl.Foundation.MvvmCross.Services;
using UIKit;

namespace Toggl.Daneel.Services
{
    public sealed class DialogService : IDialogService
    {
        public Task<bool> Confirm(
            string title, 
            string message, 
            string confirmButtonText, 
            string dismissButtonText)
        {
            var tcs = new TaskCompletionSource<bool>();

            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            var confirm = UIAlertAction.Create(confirmButtonText, UIAlertActionStyle.Default, _ => tcs.SetResult(true));
            var dismiss = UIAlertAction.Create(dismissButtonText, UIAlertActionStyle.Cancel, _ => tcs.SetResult(false));

            alert.AddAction(confirm);
            alert.AddAction(dismiss);
            alert.PreferredAction = confirm;

            getPresentationController().PresentViewController(alert, true, null);

            return tcs.Task;
        }

        private UIViewController getPresentationController()
        {
            var current = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (current.PresentedViewController != null)
                current = current.PresentedViewController;
            return current;
        }
    }
}
