using System.Linq;
using Foundation;
using ObjCRuntime;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class ReportsViewController
    {
        protected override void ConfigureKeyCommands()
        {
            base.ConfigureKeyCommands();
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

        [Export(nameof(selectWorkspace))]
        private void selectWorkspace()
        {
            ViewModel.SelectWorkspace.Execute();
        }

        [Export(nameof(selectDateRange))]
        private void selectDateRange()
        {
            ViewModel.SelectTimeRange.Execute();
        }
    }
}
