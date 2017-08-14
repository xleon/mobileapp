﻿using System.Linq;
using CoreAnimation;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.Presentation.Transition;
using Toggl.Daneel.ViewControllers;
using Toggl.Daneel.ViewControllers.Navigation;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.Presentation
{
    public class TogglPresenter : MvxIosViewPresenter
    {
        private FromBottomTransitionDelegate FromBottomTransitionDelegate = new FromBottomTransitionDelegate();

        private CATransition FadeAnimation = new CATransition
        {
            Duration = Animation.Timings.EnterTiming,
            Type = CAAnimation.TransitionFade,
            Subtype = CAAnimation.TransitionFromTop,
            TimingFunction = Animation.Curves.SharpCurve.ToMediaTimingFunction()
        };

        public TogglPresenter(IUIApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        {
        }

        protected override void RegisterAttributeTypes()
        {
            base.RegisterAttributeTypes();

            AttributeTypesToShowMethodDictionary.Add(typeof(NestedPresentationAttribute), ShowNestedViewController);
            AttributeTypesToShowMethodDictionary.Add(typeof(ModalCardPresentationAttribute), ShowModalCardViewController);
        }

        protected virtual void ShowNestedViewController(UIViewController viewController, MvxBasePresentationAttribute attribute, MvxViewModelRequest request)
        {
            var mainViewController = MasterNavigationController.ViewControllers.OfType<MainViewController>().Single();
            mainViewController.AddChildViewController(viewController);

            var container = mainViewController.GetContainerFor(viewController);
            viewController.View.Frame = new CoreGraphics.CGRect(0, 0, container.Frame.Width, container.Frame.Height);
            container.AddSubview(viewController.View);

            viewController.DidMoveToParentViewController(mainViewController);
        }

        protected virtual void ShowModalCardViewController(UIViewController viewController, MvxBasePresentationAttribute attribute, MvxViewModelRequest request)
        {
            viewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            viewController.TransitioningDelegate = FromBottomTransitionDelegate;
            MasterNavigationController.PresentViewController(viewController, true, null);

            ModalViewControllers.Add(viewController);

            FromBottomTransitionDelegate.WireToViewController(viewController, () => ModalViewControllers.Remove(viewController));
        }

        protected override void ShowChildViewController(UIViewController viewController, MvxChildPresentationAttribute attribute, MvxViewModelRequest request)
        {
            if (request.ViewModelType == typeof(LoginViewModel))
            {
                MasterNavigationController.View.Layer.AddAnimation(FadeAnimation, CALayer.Transition);
                MasterNavigationController.PushViewController(viewController, false);
                return;
            }

            base.ShowChildViewController(viewController, attribute, request);
        }

        protected override void SetWindowRootViewController(UIViewController controller)
        {
            UIView.Transition(
                _window,
                Animation.Timings.EnterTiming,
                UIViewAnimationOptions.TransitionCrossDissolve,
                () => _window.RootViewController = controller,
                () =>
                {
                    if (controller is TogglNavigationController navigation && 
                        navigation.ViewControllers.FirstOrDefault() is MainViewController mainViewController)
                        mainViewController.AnimatePlayButton();
                }
            );

        }

        public override void Close(IMvxViewModel toClose)
        {
            if (toClose.GetType() == typeof(LoginViewModel))
            {
                MasterNavigationController.View.Window.Layer.AddAnimation(FadeAnimation, CALayer.Transition);
                MasterNavigationController.PopViewController(false);
                return;
            }

            base.Close(toClose);
        }

        protected override MvxNavigationController CreateNavigationController(UIViewController viewController)
        {
            if (viewController is OnboardingViewController)
                return new OnboardingFlowNavigationController(viewController);

            return new TogglNavigationController(viewController);
        }
    }
}
