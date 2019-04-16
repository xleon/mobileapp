using CoreGraphics;
using MvvmCross.Plugin.Color.Platforms.Ios;
using Toggl.Core.UI.Helper;
using UIKit;
using static System.Math;
using System;

namespace Toggl.Daneel.Presentation.Transition
{
    public class ModalDialogPresentationController : UIPresentationController
    {
        private const double iPadMinHeightWithoutKeyboard = 360;
        private const double iPadMaxWidth = 540;
        private const double iPadTopMarginNarrow = 40;
        private const double iPadTopMarginLarge = 76;
        private const double iPadStackModalViewSpacing = 40;

        private double topiPadMargin
            => PresentingViewController.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact
               || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft
               || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight
                ? iPadTopMarginNarrow
                : iPadTopMarginLarge;

        private double iPadMaxHeight => UIScreen.MainScreen.Bounds.Height - 2 * topiPadMargin;

        private readonly UIView dimmingView = new UIView
        {
            BackgroundColor = Color.ModalDialog.BackgroundOverlay.ToNativeColor(),
            Alpha = 0
        };

        public ModalDialogPresentationController(UIViewController presentedViewController, UIViewController presentingViewController)
            : base(presentedViewController, presentingViewController)
        {

        }

        public override void PresentationTransitionWillBegin()
        {
            dimmingView.Frame = ContainerView.Bounds;
            ContainerView.AddSubview(dimmingView);

            var transitionCoordinator = PresentingViewController.GetTransitionCoordinator();
            transitionCoordinator.AnimateAlongsideTransition(context => dimmingView.Alpha = 0.8f, null);
        }

        public override void DismissalTransitionWillBegin()
        {
            var transitionCoordinator = PresentingViewController.GetTransitionCoordinator();
            transitionCoordinator.AnimateAlongsideTransition(context => dimmingView.Alpha = 0.0f, null);
        }

        public override void ContainerViewWillLayoutSubviews()
        {
            PresentedView.Layer.CornerRadius = 8;
            dimmingView.Frame = ContainerView.Bounds;
            PresentedView.Frame = FrameOfPresentedViewInContainerView;
        }

        public override CGSize GetSizeForChildContentContainer(IUIContentContainer contentContainer, CGSize parentContainerSize)
        {
            var regualrOrCompactHorizontalSizeClass = PresentingViewController.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
                                                      || PresentingViewController.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact;
            var regularVerticalSizeClass = PresentingViewController.TraitCollection.VerticalSizeClass == UIUserInterfaceSizeClass.Regular;

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad
                && regualrOrCompactHorizontalSizeClass
                && regularVerticalSizeClass)
            {
                var preferredContentHeight = PresentedViewController.PreferredContentSize.Height != 0
                    ? PresentedViewController.PreferredContentSize.Height
                    : iPadMaxHeight;

                var height = preferredContentHeight;
                var width = Min(iPadMaxWidth, parentContainerSize.Width);

                height -= iPadStackModalViewSpacing * levelsOfModalViews();

                return new CGSize(width, height);
            }

            return PresentedViewController.PreferredContentSize;
        }

        public override CGRect FrameOfPresentedViewInContainerView
        {
            get
            {
                var containerSize = ContainerView.Bounds.Size;
                var frame = CGRect.Empty;
                frame.Size = GetSizeForChildContentContainer(PresentedViewController, containerSize);
                frame.X = (containerSize.Width - frame.Width) / 2;
                frame.Y = (containerSize.Height - frame.Height) / 2;

                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                {
                    frame.Y = (containerSize.Height - frame.Size.Height) / 2 + (nfloat)iPadStackModalViewSpacing * levelsOfModalViews();

                    if (PresentingViewController.PresentingViewController != null)
                    {
                        frame.Y = (nfloat)topiPadMargin + (nfloat)iPadStackModalViewSpacing * levelsOfModalViews();
                    }
                }

                return frame;
            }
        }

        private int levelsOfModalViews()
        {
            var levels = 0;
            var topVC = PresentingViewController;
            while (topVC.PresentingViewController != null)
            {
                topVC = topVC.PresentingViewController;
                levels += 1;
            }

            return levels;
        }
    }
}
