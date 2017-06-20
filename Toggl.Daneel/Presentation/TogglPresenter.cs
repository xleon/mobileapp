using CoreAnimation;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.Presentation
{
    public class TogglPresenter : MvxIosViewPresenter
    {
        public TogglPresenter(IUIApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        {
        }

        protected override void ShowChildViewController(UIViewController viewController, MvxChildPresentationAttribute attribute, MvxViewModelRequest request)
        {
            if (request.ViewModelType != typeof(LoginViewModel))
            {
                base.ShowChildViewController(viewController, attribute, request);
                return;
            }

            var transition = new CATransition
            {
                Duration = Animation.Timings.EnterTiming,
                Type = CAAnimation.TransitionFade,
                Subtype = CAAnimation.TransitionFromTop,
                TimingFunction = Animation.Curves.SharpCurve.ToMediaTimingFunction()
            };

            MasterNavigationController.NavigationController.View.Layer.AddAnimation(transition, CALayer.Transition);
            MasterNavigationController.PushViewController(viewController, false);
        }
    }
}
