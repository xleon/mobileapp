using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.iOS.ViewControllers;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public sealed class RootPresenter : IosPresenter
    {
        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(MainTabBarViewModel),
            typeof(OnboardingViewModel),
            typeof(LoginViewModel),
            typeof(SignupViewModel),
            typeof(TokenResetViewModel),
            typeof(OutdatedAppViewModel),
        };

        public RootPresenter(UIWindow window, AppDelegate appDelegate) : base(window, appDelegate)
        {
        }

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView view)
        {
            var shouldWrapInNavigationController = !(viewModel is MainTabBarViewModel);
            UIViewController rootViewController = ViewControllerLocator.GetViewController(viewModel, shouldWrapInNavigationController);

            var oldRootViewController = Window.RootViewController;

            UIView.Transition(
                Window,
                Animation.Timings.EnterTiming,
                UIViewAnimationOptions.TransitionCrossDissolve,
                () => Window.RootViewController = rootViewController,
                () => detachOldRootViewController(oldRootViewController)
            );
        }

        private void detachOldRootViewController(UIViewController viewController)
        {
            var viewControllerToDetach = viewController is UINavigationController navigationController
                ? navigationController.ViewControllers.First()
                : viewController;

            switch (viewControllerToDetach)
            {
                case MainTabBarController mainTabBarController:
                    detachViewModel(mainTabBarController.ViewModel);
                    break;
                case OnboardingViewController onboardingViewController:
                    detachViewModel(onboardingViewController.ViewModel);
                    break;
                case LoginViewController loginViewController:
                    detachViewModel(loginViewController.ViewModel);
                    break;
                case SignupViewController signupViewController:
                    detachViewModel(signupViewController.ViewModel);
                    break;
                case TokenResetViewController tokenResetViewController:
                    detachViewModel(tokenResetViewController.ViewModel);
                    break;
                case OutdatedAppViewController outdatedAppViewController:
                    detachViewModel(outdatedAppViewController.ViewModel);
                    break;
            }
        }

        private void detachViewModel<TViewModel>(TViewModel viewModel)
            where TViewModel : IViewModel
        {
            viewModel?.Cancel();
            viewModel?.DetachView();
            viewModel?.ViewDestroyed();
        }
    }
}
