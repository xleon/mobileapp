using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public class GroupBindingTableViewSource<TKey, TType> : MvxSimpleTableViewSource
    {
        private readonly string headerCellIdentifier;

        private IEnumerable<IGrouping<TKey, TType>> GroupedItems
            => ItemsSource as IEnumerable<IGrouping<TKey, TType>>;

        public GroupBindingTableViewSource(UITableView tableView, string headerCellIdentifier, string cellIdentifier)
            : base(tableView, cellIdentifier)
        {
            this.headerCellIdentifier = headerCellIdentifier;

            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var grouping = GetGroupAt(section);

            var cell = GetOrCreateHeaderViewFor(tableView);
            if (cell is IMvxBindable bindable)
                bindable.DataContext = grouping;
            
            return cell;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            var cell = GetOrCreateCellFor(tableView, indexPath, item);
            if (cell is IMvxBindable bindable)
                bindable.DataContext = item;
            
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
            => ItemsSource.Count();

        public override nint RowsInSection(UITableView tableview, nint section)
            => GetGroupAt(section).Count();

        protected virtual IGrouping<TKey, TType> GetGroupAt(nint section)
            => GroupedItems.ElementAt((int)section);

        protected override object GetItemAt(NSIndexPath indexPath)
            => GroupedItems.ElementAt(indexPath.Section).ElementAt((int)indexPath.Item);

        protected UITableViewHeaderFooterView GetOrCreateHeaderViewFor(UITableView tableView)
            => tableView.DequeueReusableHeaderFooterView(headerCellIdentifier);

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
            => tableView.DequeueReusableCell(CellIdentifier, indexPath);

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) => 43;

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 64;
    }
}
