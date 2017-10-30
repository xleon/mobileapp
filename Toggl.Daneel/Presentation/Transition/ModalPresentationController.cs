using System;
using CoreGraphics;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Multivac;
using UIKit;
using static System.Math;

namespace Toggl.Daneel.Presentation.Transition
{
    public sealed class ModalPresentationController : UIPresentationController
    {
        private readonly Action onDismissedCallback;
        private readonly nfloat maximumHeight = UIScreen.MainScreen.Bounds.Height - 20;

        private readonly UIView dimmingView = new UIView
        {
            BackgroundColor = Color.ModalDialog.BackgroundOverlay.ToNativeColor(),
            Alpha = 0
        };

        public UIView AdditionalContentView { get; }
            = new UIView();

        public ModalPresentationController(UIViewController presentedViewController,
            UIViewController presentingViewController, Action onDismissedCallback)
          : base(presentedViewController, presentingViewController)
        {
            Ensure.Argument.IsNotNull(onDismissedCallback, nameof(onDismissedCallback));

            this.onDismissedCallback = onDismissedCallback;

            var recognizer = new UITapGestureRecognizer(dismiss);
            AdditionalContentView.AddGestureRecognizer(recognizer);
        }

        public override void PresentationTransitionWillBegin()
        {
            dimmingView.Frame = ContainerView.Bounds;
            AdditionalContentView.Frame = ContainerView.Bounds;
            AdditionalContentView.Layer.ZPosition += 1;

            ContainerView.AddSubview(dimmingView);
            ContainerView.AddSubview(AdditionalContentView);

            var coordinator = PresentedViewController.GetTransitionCoordinator();
            if (coordinator == null)
            {
                dimmingView.Alpha = 0.8f;
                return;
            }

            coordinator.AnimateAlongsideTransition(_ => dimmingView.Alpha = 0.8f, null);
        }

        public override void DismissalTransitionWillBegin()
        {
            var coordinator = PresentedViewController.GetTransitionCoordinator();
            if (coordinator == null)
            {
                dimmingView.Alpha = 0.0f;
                return;
            }

            coordinator.AnimateAlongsideTransition(_ => dimmingView.Alpha = 0.0f, null);
        }

        public override void ContainerViewWillLayoutSubviews()
        {
            PresentedView.Frame = FrameOfPresentedViewInContainerView;

            var shadowPath = UIBezierPath.FromRoundedRect(PresentedViewController.View.Bounds, 8.0f).CGPath;
            PresentedViewController.View.Layer.ShadowRadius = 20;
            PresentedViewController.View.Layer.CornerRadius = 8.0f;
            PresentedViewController.View.Layer.ShadowOpacity = 0.8f;
            PresentedViewController.View.Layer.MasksToBounds = false;
            PresentedViewController.View.Layer.ShadowPath = shadowPath;
            PresentedViewController.View.Layer.ShadowOffset = CGSize.Empty;
            PresentedViewController.View.Layer.ShadowColor = UIColor.FromRGB(181f / 255f, 188f / 255f, 192f / 255f).CGColor;
        }

        public override CGSize GetSizeForChildContentContainer(IUIContentContainer contentContainer, CGSize parentContainerSize)
        {
            var preferredHeight = Min(maximumHeight, PresentedViewController.PreferredContentSize.Height);
            return new CGSize(parentContainerSize.Width, preferredHeight == 0 ? maximumHeight : preferredHeight);
        }

        public override CGRect FrameOfPresentedViewInContainerView
        {
            get
            {
                var containerSize = ContainerView.Bounds.Size;
                var frame = CGRect.Empty;
                frame.Size = GetSizeForChildContentContainer(PresentedViewController, containerSize);
                frame.X = 0;
                frame.Y = containerSize.Height - frame.Size.Height;
                return frame;
            }
        }

        private void dismiss()
        {
            PresentedViewController.DismissViewController(true, null);
            onDismissedCallback();
        }
    }
}
