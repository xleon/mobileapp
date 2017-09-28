using System;
using Foundation;
using Toggl.Daneel.Views;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class SelectTagsTableViewSource : GroupedCollectionTableViewSource<SelectableTagViewModel>
    {
        private const string cellIdentifier = nameof(SelectableTagViewCell);
        private const string headerCellIdentifier = nameof(WorkspaceHeaderViewCell);

        public SelectTagsTableViewSource(UITableView tableView) 
            : base(tableView, cellIdentifier, headerCellIdentifier)
        {
            tableView.RegisterNibForCellReuse(SelectableTagViewCell.Nib, cellIdentifier);
            tableView.RegisterNibForHeaderFooterViewReuse(WorkspaceHeaderViewCell.Nib, headerCellIdentifier);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 48;

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) => 40;
    }
}
