using System.Threading.Tasks;
using CoreGraphics;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.ViewSources;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.Daneel.Extensions.Reactive;
using UIKit;
using System;
using System.Linq;
using System.Reactive.Linq;
using static Toggl.Shared.Extensions.ReactiveExtensions;

namespace Toggl.Daneel.ViewControllers
{
    [ModalCardPresentation]
    public sealed partial class SelectProjectViewController : KeyboardAwareViewController<SelectProjectViewModel>, IDismissableViewController
    {
        private const double headerHeight = 99;
        private const double placeHolderHeight = 250;

        public SelectProjectViewController()
            : base(nameof(SelectProjectViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new SelectProjectTableViewSource();
            source.RegisterViewCells(ProjectsTableView);

            source.UseGrouping = ViewModel.UseGrouping;

            ProjectsTableView.TableFooterView = new UIView();
            ProjectsTableView.Source = source;

            var suggestionsReplay = ViewModel.Suggestions.Replay();

            suggestionsReplay
                .Subscribe(ProjectsTableView.Rx().ReloadSections(source))
                .DisposedBy(DisposeBag);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                suggestionsReplay
                    .Select((sections) =>
                    {
                        var numberOfSections = sections.ToList().Count();
                        var numberOfSuggestions = sections.Select(s => s.Items.Count()).Sum();
                        return (numberOfSections, numberOfSuggestions);
                    })
                    .Select((result) =>
                    {
                        var (numberOfSections, numberOfSuggestions) = result;
                        var headersHeight = ViewModel.UseGrouping
                            ? numberOfSections * SelectProjectTableViewSource.HeaderHeight
                            : 0;
                        var suggestionsHeight = numberOfSuggestions * SelectProjectTableViewSource.RowHeight;
                        var contentHeight = numberOfSuggestions == 1
                            ? placeHolderHeight
                            : headersHeight + suggestionsHeight;
                        return new CGSize(0, contentHeight + headerHeight);
                    })
                    .Subscribe(this.Rx().PreferredContentSize())
                    .DisposedBy(DisposeBag);
            }

            suggestionsReplay.Connect();

            ViewModel.IsEmpty
                .Subscribe(EmptyStateLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsEmpty
                .Subscribe(EmptyStateImage.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.PlaceholderText
                .Subscribe(TextField.Rx().PlaceholderText())
                .DisposedBy(DisposeBag);

            TextField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            CloseButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectProject.Inputs)
                .DisposedBy(DisposeBag);

            source.ToggleTaskSuggestions
                .Subscribe(ViewModel.ToggleTaskSuggestions.Inputs)
                .DisposedBy(DisposeBag);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TextField.BecomeFirstResponder();

            BottomConstraint.Active |= UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad;
        }

        public async Task<bool> Dismiss()
        {
            ViewModel.Close.Execute();
            return true;
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
