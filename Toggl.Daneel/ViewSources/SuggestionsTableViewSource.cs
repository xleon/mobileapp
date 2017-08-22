using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Binding.iOS.Views;
using Toggl.Daneel.Views;
using Toggl.Foundation;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class SuggestionsTableViewSource : MvxTableViewSource
    {
        private const string cellIdentifier = nameof(SuggestionsViewCell);
        private const string emptyCellIdentifier = nameof(SuggestionsEmptyViewCell);

        public SuggestionsTableViewSource(UITableView tableView)
            : base(tableView)
        {
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.RegisterNibForCellReuse(SuggestionsViewCell.Nib, cellIdentifier);
            tableView.RegisterNibForCellReuse(SuggestionsEmptyViewCell.Nib, emptyCellIdentifier);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var totalWidth = UIScreen.MainScreen.Bounds.Width;
            return new UIView(new CGRect(0, 0, totalWidth, 45))
            {
                new UILabel(new CGRect(16, 16, totalWidth - 16, 19))
                {
                    Font = UIFont.SystemFontOfSize(16, UIFontWeight.Medium),
                    Text = Resources.SuggestionsHeader
                }
            };
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var cell = GetOrCreateCellFor(tableView, indexPath, item);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            if (item != null && cell is IMvxBindable bindable)
                bindable.DataContext = item;

            return cell;
        }

        public override nint NumberOfSections(UITableView tableView) => 1;

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            var count = ItemsSource.Count();
            if (count == 0) return 3;

            return count;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            if (ItemsSource.Count() == 0) return null;

            return ItemsSource.ElementAt((int)indexPath.Item);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
            => tableView.DequeueReusableCell(item == null ? emptyCellIdentifier : cellIdentifier, indexPath);

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 64;

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) => 45;
    }
}
