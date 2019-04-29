using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Presentation.Attributes;
using Toggl.iOS.Views;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    [ModalCardPresentation]
    public partial class SelectWorkspaceViewController : ReactiveViewController<SelectWorkspaceViewModel>, IDismissableViewController
    {
        private const int rowHeight = 64;
        private const double headerHeight = 54;

        public SelectWorkspaceViewController()
            : base(nameof(SelectWorkspaceViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            WorkspaceTableView.RowHeight = rowHeight;
            WorkspaceTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            WorkspaceTableView.RegisterNibForCellReuse(WorkspaceViewCell.Nib, WorkspaceViewCell.Identifier);

            var source = new CustomTableViewSource<SectionModel<Unit, SelectableWorkspaceViewModel>, Unit, SelectableWorkspaceViewModel>(
                WorkspaceViewCell.CellConfiguration(WorkspaceViewCell.Identifier),
                ViewModel.Workspaces
            );
            WorkspaceTableView.Source = source;

            TitleLabel.Text = ViewModel.Title;

            CloseButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectWorkspace.Inputs)
                .DisposedBy(DisposeBag);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                PreferredContentSize = new CoreGraphics.CGSize(0, headerHeight + (ViewModel.Workspaces.Count * rowHeight));
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            View.ClipsToBounds |= UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            View.ClipsToBounds |= UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
        }

        public async Task<bool> Dismiss()
        {
            await ViewModel.Close.ExecuteWithCompletion();
            return true;
        }
    }
}

