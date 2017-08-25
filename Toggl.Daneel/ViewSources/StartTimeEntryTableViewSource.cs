using System;
using Foundation;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Views;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class StartTimeEntryTableViewSource : MvxTableViewSource
    {
        private const string timeEntryCellIdentifier = nameof(StartTimeEntryViewCell);
        private const string projectCellIdentifier = nameof(StartTimeEntryProjectsViewCell);

        public StartTimeEntryTableViewSource(UITableView tableView)
            : base(tableView)
        {
            tableView.TableFooterView = new UIView();
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
            tableView.SeparatorColor = Color.StartTimeEntry.SeparatorColor.ToNativeColor();
            tableView.RegisterNibForCellReuse(StartTimeEntryViewCell.Nib, timeEntryCellIdentifier);
            tableView.RegisterNibForCellReuse(StartTimeEntryProjectsViewCell.Nib, projectCellIdentifier);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var cell = GetOrCreateCellFor(tableView, indexPath, item);
            cell.LayoutMargins = UIEdgeInsets.Zero;
            cell.SeparatorInset = UIEdgeInsets.Zero;
            cell.PreservesSuperviewLayoutMargins = false;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            if (cell is IMvxBindable bindable)
                bindable.DataContext = item;

            return cell;
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
            => tableView.DequeueReusableCell(getIdentifier(item), indexPath);

        private string getIdentifier(object item)
        {
            if (item is ProjectSuggestionViewModel)
                return projectCellIdentifier;

            return timeEntryCellIdentifier;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 48;
    }
}
