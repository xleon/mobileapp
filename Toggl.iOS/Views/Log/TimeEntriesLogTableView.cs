using Foundation;
using System;
using Toggl.iOS.Cells.MainLog;
using Toggl.iOS.ViewSources;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register("TimeEntriesLogTableView")]
    public class TimeEntriesLogTableView : UITableView
    {
        private RefreshControl customRefreshControl;
        public RefreshControl CustomRefreshControl
        {
            get => customRefreshControl;
            set
            {
                customRefreshControl = value;
                customRefreshControl.Configure(this);
            }
        }

        public TimeEntriesLogTableView(IntPtr p) : base(p)
        {
            SeparatorStyle = UITableViewCellSeparatorStyle.None;
            RegisterNibForCellReuse(SuggestionLogViewCell.Nib, SuggestionLogViewCell.Identifier);
            RegisterNibForHeaderFooterViewReuse(SuggestionsHeaderView.Nib, SuggestionsHeaderView.Identifier);
            RegisterNibForCellReuse(UserFeedbackTableViewCell.Nib, UserFeedbackTableViewCell.Identifier);
            RegisterNibForCellReuse(TimeEntriesLogViewCell.Nib, TimeEntriesLogViewCell.Identifier);
            RegisterNibForHeaderFooterViewReuse(TimeEntriesLogHeaderView.Nib, TimeEntriesLogHeaderView.Identifier);
            TableFooterView = new UIView();
        }
    }
}
