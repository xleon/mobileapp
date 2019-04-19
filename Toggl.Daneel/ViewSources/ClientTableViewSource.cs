using Foundation;
using Toggl.Daneel.Cells;
using Toggl.Daneel.Views.Client;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    using ClientSection = SectionModel<string, SelectableClientBaseViewModel>;

    public sealed class ClientTableViewSource : BaseTableViewSource<ClientSection, string, SelectableClientBaseViewModel>
    {
        public const int RowHeight = 48;

        public ClientTableViewSource(UITableView tableView)
        {
            tableView.RowHeight = RowHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var model = ModelAt(indexPath);
            var identifier = model is SelectableClientCreationViewModel ? CreateClientViewCell.Identifier : ClientViewCell.Identifier;
            var cell = (BaseTableViewCell<SelectableClientBaseViewModel>)tableView.DequeueReusableCell(identifier);
            cell.Item = model;
            return cell;
        }
    }
}
