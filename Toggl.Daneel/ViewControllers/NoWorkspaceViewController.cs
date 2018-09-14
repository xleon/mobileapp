using System.Reactive.Linq;
using CoreGraphics;
using MvvmCross.Plugin.Color.Platforms.Ios;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Extensions.Reactive;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac.Extensions;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [ModalDialogPresentation]
    public sealed partial class NoWorkspaceViewController
        : ReactiveViewController<NoWorkspaceViewModel>
    {
        private const float cardHeight = 368;

        public NoWorkspaceViewController() : base(nameof(NoWorkspaceViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                // ScreenWidth - 32 for 16pt margins on both sides
                Width = screenWidth > 320 ? screenWidth - 32 : 312,
                Height = cardHeight
            };

            prepareViews();

            this.Bind(CreateWorkspaceButton.Rx().Tap(), ViewModel.CreateWorkspaceWithDefaultName);
            this.Bind(TryAgainButton.Rx().Tap(), ViewModel.TryAgain);

            this.Bind(ViewModel.IsLoading.Select(CommonFunctions.Invert), CreateWorkspaceButton.Rx().Enabled());
            this.Bind(ViewModel.IsLoading.Select(CommonFunctions.Invert), TryAgainButton.Rx().IsVisibleWithFade());
            this.Bind(ViewModel.IsLoading.StartWith(false), ActivityIndicatorView.Rx().IsVisibleWithFade());
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ActivityIndicatorView.StartSpinning();
        }

        private void prepareViews()
        {
            ActivityIndicatorView.IndicatorColor = Color.NoWorkspace.ActivityIndicator.ToNativeColor();
            CreateWorkspaceButton.SetTitleColor(Color.NoWorkspace.DisabledCreateWorkspaceButton.ToNativeColor(), UIControlState.Disabled);
        }
    }
}
