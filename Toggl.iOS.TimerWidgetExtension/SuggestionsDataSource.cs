using System;
using Foundation;
using UIKit;

namespace Toggl.iOS.TimerWidgetExtension
{
    public class SuggestionsDataSource : UITableViewDataSource
    {
        private const string identifier = "SuggestionCell";

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(identifier, indexPath) as SuggestionTableViewCell;
            return cell;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return 3;
        }
    }
}
