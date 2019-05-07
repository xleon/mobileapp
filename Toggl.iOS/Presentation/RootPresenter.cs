using System;
using System.Collections.Generic;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
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

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel)
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
            UIView.Transition(
                Window,
                Animation.Timings.EnterTiming,
                UIViewAnimationOptions.TransitionCrossDissolve,
                () => Window.RootViewController = controller,
                null
            );
        }

        private UINavigationController wrapInNavigationController(UIViewController viewController)
            => new UINavigationController(viewController);
    }
}
