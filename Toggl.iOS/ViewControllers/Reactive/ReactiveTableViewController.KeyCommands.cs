using Foundation;
using ObjCRuntime;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class ReactiveTableViewController<TViewModel>
    {
        protected readonly UIKeyCommand ShowMainLogKeyCommand = UIKeyCommand.Create(
            title: Resources.Timer,
            image: null,
            action: new Selector(nameof(showMainLog)),
            input: "1",
            modifierFlags: UIKeyModifierFlags.Command,
            propertyList: null);

        protected readonly UIKeyCommand ShowReportsKeyCommand = UIKeyCommand.Create(
            title: Resources.Reports,
            image: null,
            action: new Selector(nameof(showReports)),
            input: "2",
            modifierFlags: UIKeyModifierFlags.Command,
            propertyList: null);

        protected readonly UIKeyCommand ShowCalendarKeyCommand = UIKeyCommand.Create(
            title: Resources.Calendar,
            image: null,
            action: new Selector(nameof(showCalendar)),
            input: "3",
            modifierFlags: UIKeyModifierFlags.Command,
            propertyList: null);

        [Export(nameof(showMainLog))]
        private void showMainLog()
        {
            TabBarController.SelectedIndex = 0;
        }

        [Export(nameof(showReports))]
        private void showReports()
        {
            TabBarController.SelectedIndex = 1;
        }

        [Export(nameof(showCalendar))]
        private void showCalendar()
        {
            TabBarController.SelectedIndex = 2;
        }
    }
}
