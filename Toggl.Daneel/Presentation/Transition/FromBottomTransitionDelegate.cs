using System;
using Foundation;
using Toggl.Multivac;
using UIKit;

namespace Toggl.Daneel.Presentation.Transition
{
    public sealed class FromBottomTransitionDelegate : NSObject, IUIViewControllerTransitioningDelegate
    {
        private readonly Action onDismissedCallback;

        public FromBottomTransitionDelegate(Action onDismissedCallback)
        {
            Ensure.Argument.IsNotNull(onDismissedCallback, nameof(onDismissedCallback));

            this.onDismissedCallback = onDismissedCallback;
        }

        private readonly SwipeInteractionController swipeInteractionController = new SwipeInteractionController();

        [Export("animationControllerForPresentedController:presentingController:sourceController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(
            UIViewController presented, UIViewController presenting, UIViewController source
        ) => new FromBottomTransition(true);

        [Export("animationControllerForDismissedController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
            => new FromBottomTransition(false);

        [Export("presentationControllerForPresentedViewController:presentingViewController:sourceViewController:")]
        public UIPresentationController GetPresentationControllerForPresentedViewController(
            UIViewController presented, UIViewController presenting, UIViewController source
        ) => new ModalPresentationController(presented, presenting, onDismissedCallback);

        [Export("interactionControllerForDismissal:")]
        public IUIViewControllerInteractiveTransitioning GetInteractionControllerForDismissal(IUIViewControllerAnimatedTransitioning animator)
            => swipeInteractionController.InteractionInProgress ? swipeInteractionController : null;

        public void WireToViewController(UIViewController vc)
        => swipeInteractionController.WireToViewController(vc, onDismissedCallback);
    }
}
