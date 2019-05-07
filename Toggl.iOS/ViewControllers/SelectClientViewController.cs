using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Core;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Views.Client;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class SelectClientViewController : KeyboardAwareViewController<SelectClientViewModel>, IDismissableViewController
    {
        private const double headerHeight = 100;

        public SelectClientViewController()
            : base(nameof(SelectClientViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TitleLabel.Text = Resources.Clients;
            SearchTextField.Placeholder = Resources.AddFilterClients;

            SuggestionsTableView.RegisterNibForCellReuse(ClientViewCell.Nib, ClientViewCell.Identifier);
            SuggestionsTableView.RegisterNibForCellReuse(CreateClientViewCell.Nib, CreateClientViewCell.Identifier);
            SuggestionsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            var tableViewSource = new ClientTableViewSource(SuggestionsTableView);
            SuggestionsTableView.Source = tableViewSource;

            var clientsReplay = ViewModel.Clients.Replay();

            clientsReplay
                .Subscribe(SuggestionsTableView.Rx().ReloadItems(tableViewSource))
                .DisposedBy(DisposeBag);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                clientsReplay
                    .Select((clients) =>
                    {
                        return new CoreGraphics.CGSize(0, (clients.ToList().Count() * ClientTableViewSource.RowHeight) + headerHeight);
                    })
                    .Subscribe(this.Rx().PreferredContentSize())
                    .DisposedBy(DisposeBag);
            }

            clientsReplay.Connect();

            CloseButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);

            SearchTextField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectClient.Inputs)
                .DisposedBy(DisposeBag);

            BottomConstraint.Active |= UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad;
        }

        public async Task<bool> Dismiss()
        {
            ViewModel.Close.Execute(Unit.Default);
            return true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SearchTextField.BecomeFirstResponder();
        }

        protected override void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.Constant = e.FrameEnd.Height;
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        protected override void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.Constant = 0;
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
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
    }
}
