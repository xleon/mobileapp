using System;
using CoreGraphics;
using UIKit;

namespace Toggl.Daneel.Presentation.Transition
{
    public class ModalPresentationController : UIPresentationController
    {
        private readonly nfloat defaultHeight = UIScreen.MainScreen.Bounds.Height - 20;

        private readonly UIView dimmingView = new UIView
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            BackgroundColor = UIColor.FromWhiteAlpha(1.0f, 0.8f),
            Alpha = 0.0f
        };

        public ModalPresentationController(UIViewController presentedViewController, UIViewController presentingViewController)
          : base(presentedViewController, presentingViewController)
        {
            var recognizer = new UITapGestureRecognizer(dismiss);
            dimmingView.AddGestureRecognizer(recognizer);
        }

        private void dismiss()
        {
            PresentedViewController.DismissViewController(true, null);
        }

        public override void PresentationTransitionWillBegin()
        {
            ContainerView.InsertSubview(dimmingView, 0);

            var coordinator = PresentedViewController.GetTransitionCoordinator();
            if (coordinator == null)
            {
                dimmingView.Alpha = 1.0f;
                return;
            }

            coordinator.AnimateAlongsideTransition(_ => dimmingView.Alpha = 1.0f, null);
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
            var preferredHeight = PresentedViewController.PreferredContentSize.Height;
            return new CGSize(parentContainerSize.Width, preferredHeight == 0 ? defaultHeight : preferredHeight);
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
    }
}
