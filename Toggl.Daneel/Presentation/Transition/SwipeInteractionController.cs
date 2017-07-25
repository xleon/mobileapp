using Toggl.Multivac;
using UIKit;
using static UIKit.UIGestureRecognizerState;

namespace Toggl.Daneel.Presentation.Transition
{
    public class SwipeInteractionController : UIPercentDrivenInteractiveTransition
    {
        private UIViewController viewController;
        private bool shouldCompleteTransition = false;

        public bool InteractionInProgress { get; set; } = false;

        public void WireToViewController(UIViewController viewController)
        {
            Ensure.Argument.IsNotNull(viewController, nameof(viewController));

            this.viewController = viewController;
            prepareGestureRecognizerInView(viewController.View);
        }

        private void prepareGestureRecognizerInView(UIView view)
        {
            var gesture = new UIPanGestureRecognizer(handleGesture);
            view.AddGestureRecognizer(gesture);
        }

        private void handleGesture(UIPanGestureRecognizer recognizer)
        {
            var translation = recognizer.TranslationInView(recognizer.View.Superview);
            var percent = translation.Y / recognizer.View.Superview.Bounds.Size.Height;

            switch (recognizer.State)
            {
                case Began:
                    InteractionInProgress = true;
                    viewController.DismissViewController(true, null);
                    break;

                case Changed:
                    shouldCompleteTransition = percent > 0.2;
                    UpdateInteractiveTransition(percent);
                    break;

                case Cancelled:
                    InteractionInProgress = false;
                    CancelInteractiveTransition();
                    break;

                case Ended:
                    InteractionInProgress = false;
                    if (!shouldCompleteTransition) 
                    {
                        CancelInteractiveTransition();
                        return;
                    }

                    FinishInteractiveTransition();
                    break;
            }
        }
    }
}
