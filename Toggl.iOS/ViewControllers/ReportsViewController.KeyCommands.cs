using System.Linq;
using Foundation;
using ObjCRuntime;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class ReportsViewController
    {
        protected override void ConfigureKeyCommands()
        {
            base.ConfigureKeyCommands();
            AddKeyCommand(shortcutCommand("1", Resources.Today));
            AddKeyCommand(shortcutCommand("2", Resources.Yesterday));
            AddKeyCommand(shortcutCommand("3", Resources.ThisWeek));
            AddKeyCommand(shortcutCommand("4", Resources.LastWeek));
            AddKeyCommand(shortcutCommand("5", Resources.ThisMonth));
            AddKeyCommand(shortcutCommand("6", Resources.LastMonth));
            AddKeyCommand(shortcutCommand("7", Resources.ThisYear));
            AddKeyCommand(shortcutCommand("8", Resources.LastYear));
            AddKeyCommand(selectDateRangeCommand);
            AddKeyCommand(selectWorkspaceCommand);
            AddKeyCommand(ShowMainLogKeyCommand);
            AddKeyCommand(ShowReportsKeyCommand);
            AddKeyCommand(ShowCalendarKeyCommand);
        }

        private readonly UIKeyCommand selectWorkspaceCommand = UIKeyCommand.Create(
            title: Resources.Workspace,
            image: null,
            action: new Selector(nameof(selectWorkspace)),
            input: "w",
            modifierFlags: UIKeyModifierFlags.Command,
            propertyList: null);

        private readonly UIKeyCommand selectDateRangeCommand = UIKeyCommand.Create(
            title: Resources.DateRange,
            image: null,
            action: new Selector(nameof(selectDateRange)),
            input: "d",
            modifierFlags: UIKeyModifierFlags.Command,
            propertyList: null);

        private UIKeyCommand shortcutCommand(string input, string title)
            => UIKeyCommand.Create(
            title: title,
            image: null,
            action: new Selector("selectShortcut:"),
            input: input,
            modifierFlags: UIKeyModifierFlags.Command | UIKeyModifierFlags.Alternate,
            propertyList: null);

        [Export(nameof(selectWorkspace))]
        private void selectWorkspace()
        {
            ViewModel.SelectWorkspace.Execute();
        }

        [Export(nameof(selectDateRange))]
        private void selectDateRange()
        {
            toggleCalendar();
        }

        [Export("selectShortcut:")]
        private void selectShortcut(UIKeyCommand command)
        {
            var title = command.Title;
            if (title == null)
                return;

            var shortcut = ViewModel.CalendarViewModel.QuickSelectShortcuts.FirstOrDefault(s => s.Title == title);
            if (shortcut != null)
            {
                ViewModel.CalendarViewModel.SelectShortcut.Execute(shortcut);
            }
        }
    }
}
