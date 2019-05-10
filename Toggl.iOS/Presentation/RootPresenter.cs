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
    public class RootPresenter : IosPresenter
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
            UIViewController rootViewController = null;

            switch (viewModel)
            {
                case MainTabBarViewModel mainTabBarViewModel:
                    var mainTabBarController = new MainTabBarController(mainTabBarViewModel);
                    rootViewController = mainTabBarController;
                    break;
                case OnboardingViewModel onboardingViewModel:
                    var onboardingViewController = new OnboardingViewController();
                    onboardingViewController.ViewModel = onboardingViewModel;
                    rootViewController = wrapInNavigationController(onboardingViewController);
                    break;
                case LoginViewModel loginViewModel:
                    var loginViewController = LoginViewController.NewInstance();
                    loginViewController.ViewModel = loginViewModel;
                    rootViewController = wrapInNavigationController(loginViewController);
                    break;
                case SignupViewModel signupViewModel:
                    var signupViewController = new SignupViewController();
                    signupViewController.ViewModel = signupViewModel;
                    rootViewController = wrapInNavigationController(signupViewController);
                    break;
                case TokenResetViewModel tokenResetViewModel:
                    var tokenResetViewController = new TokenResetViewController();
                    tokenResetViewController.ViewModel = tokenResetViewModel;
                    rootViewController = wrapInNavigationController(tokenResetViewController);
                    break;
                case OutdatedAppViewModel outdatedAppViewModel:
                    var outdatedAppViewController = new OutdatedAppViewController();
                    outdatedAppViewController.ViewModel = outdatedAppViewModel;
                    rootViewController = wrapInNavigationController(outdatedAppViewController);
                    break;
            }

            if (rootViewController == null)
                throw new Exception($"Failed to create ViewController for ViewModel of type {viewModel.GetType().Name}");

            setRootViewController(rootViewController);
        }

        private void setRootViewController(UIViewController controller)
        {
            var oldRootViewController = Window.RootViewController;

            UIView.Transition(
                Window,
                Animation.Timings.EnterTiming,
                UIViewAnimationOptions.TransitionCrossDissolve,
                () => Window.RootViewController = controller,
                () => detachOldRootViewController(oldRootViewController)
            );
        }

        private UINavigationController wrapInNavigationController(UIViewController viewController)
            => new UINavigationController(viewController);

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
