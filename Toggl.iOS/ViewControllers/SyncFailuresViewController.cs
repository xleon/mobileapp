using System.Reactive;
using UIKit;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.Models;
using Toggl.Core.UI.Collections;
using Toggl.iOS.Cells;
using Toggl.iOS.ViewSources.Generic.TableView;

namespace Toggl.iOS.ViewControllers
{
    [MvxChildPresentation]
    public sealed class SyncFailuresViewController : ReactiveTableViewController<SyncFailuresViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.RegisterNibForCellReuse(SyncFailureCell.Nib, SyncFailureCell.Identifier);
            var tableViewSource = new CustomTableViewSource<SectionModel<Unit, SyncFailureItem>, Unit, SyncFailureItem>(
                SyncFailureCell.CellConfiguration(SyncFailureCell.Identifier),
                ViewModel.SyncFailures
            );
            TableView.Source = tableViewSource;
        }
    }
}
