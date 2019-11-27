using System;
using System.Reactive.Linq;
using CoreGraphics;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Presentation;
using Toggl.iOS.Views.Reports;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class ReportsViewController : ReactiveViewController<ReportsViewModel>, IScrollableToTop
    {
        private UIButton titleButton;

        private ReportsCollectionViewRegularLayout regularLayout;
        private ReportsCollectionViewCompactLayout compactLayout;

        public ReportsViewController(ReportsViewModel viewModel) : base(viewModel, nameof(ReportsViewController))
        {
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
                CollectionView.SetCollectionViewLayout(regularLayout, false);
            else
                CollectionView.SetCollectionViewLayout(compactLayout, false);
            CollectionView.CollectionViewLayout.InvalidateLayout();
            base.TraitCollectionDidChange(previousTraitCollection);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            prepareViews();

            ViewModel.CurrentWorkspaceName
                .Subscribe(WorkspaceLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            ViewModel.FormattedTimeRange
                .Subscribe(titleButton.Rx().TitleAdaptive())
                .DisposedBy(DisposeBag);

            ViewModel.HasMultipleWorkspaces
                .Subscribe(WorkspaceButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            WorkspaceButton.Rx()
                .BindAction(ViewModel.SelectWorkspace)
                .DisposedBy(DisposeBag);

            var source = new ReportsCollectionViewSource(CollectionView);
            CollectionView.Source = source;
            regularLayout = new ReportsCollectionViewRegularLayout(source);
            compactLayout = new ReportsCollectionViewCompactLayout(source);
            if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
                CollectionView.SetCollectionViewLayout(regularLayout, false);
            else
                CollectionView.SetCollectionViewLayout(compactLayout, false);

            ViewModel.Elements
                .Subscribe(source.SetNewElements)
                .DisposedBy(DisposeBag);
        }

        public void ScrollToTop() { }

        private void prepareViews()
        {
            var separator = NavigationController.NavigationBar.InsertSeparator();
            separator.BackgroundColor = ColorAssets.OpaqueSeparator;

            // Date range button
            NavigationItem.TitleView = titleButton = new UIButton(new CGRect(0, 0, 200, 40));
            titleButton.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium);
            titleButton.SetTitleColor(ColorAssets.Text, UIControlState.Normal);

            // Workspace button settings
            WorkspaceFadeView.FadeWidth = 32;
            WorkspaceButton.Layer.ShadowColor = UIColor.Black.CGColor;
            WorkspaceButton.Layer.ShadowRadius = 10;
            WorkspaceButton.Layer.ShadowOffset = new CGSize(0, 2);
            WorkspaceButton.Layer.ShadowOpacity = 0.10f;
            WorkspaceButton.Layer.BorderColor = ColorAssets.Separator.CGColor;
            WorkspaceButton.Layer.BorderWidth = 0.35f;

            View.BackgroundColor = ColorAssets.TableBackground;
        }
    }
}

