#if !USE_PRODUCTION_API
using Toggl.iOS.DebugHelpers;
using Toggl.iOS.Presentation.Transition;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class SettingsViewController
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            AboutView.AddGestureRecognizer(new UILongPressGestureRecognizer(recognizer =>
            {
                if (recognizer.State != UIGestureRecognizerState.Recognized)
                    return;

                showErrorTriggeringView();
            }));
        }

        private void showErrorTriggeringView()
        {
            PresentViewController(new ErrorTriggeringViewController
            {
                ModalPresentationStyle = UIModalPresentationStyle.Custom,
                TransitioningDelegate = new ModalDialogTransitionDelegate()
            }, true, null);
        }
    }
}
#endif
