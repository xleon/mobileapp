using Xamarin.UITest;

namespace Toggl.Daneel.Tests.UI.Extensions
{
    public static class LoginExtensions
    {
        public static void WaitForLoginScreen(this IApp app)
        {
            app.WaitForOnboardingScreen();
            app.SkipToLastOnboardingPage();
            app.OpenLoginFromOnboardingLastPage();
        }

        public static void GoToPasswordScreen(this IApp app)
        {
            app.Tap(Login.NextButton);
            app.WaitForElement(Login.PasswordText);
        }

        public static void GoBackToOnboardingScreen(this IApp app)
        {
            app.Tap(Login.BackButton);
            app.WaitForElement(Onboarding.LoginButton);
        }

        public static void GoBackToEmailScreen(this IApp app)
        {
            app.Tap(Login.BackButton);
            app.WaitForElement(Login.EmailText);
        }
    }
}
