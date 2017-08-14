using System;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Presentation.Transition
{
    public class FromBottomTransitionDelegate : NSObject, IUIViewControllerTransitioningDelegate
    {
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
        ) => new ModalPresentationController(presented, presenting);

        [Export("interactionControllerForDismissal:")]
        public IUIViewControllerInteractiveTransitioning GetInteractionControllerForDismissal(IUIViewControllerAnimatedTransitioning animator)
            => swipeInteractionController.InteractionInProgress ? swipeInteractionController : null;

        public void WireToViewController(UIViewController vc, Action onCompletedCallback)
            => swipeInteractionController.WireToViewController(vc, onCompletedCallback);
    }
}
