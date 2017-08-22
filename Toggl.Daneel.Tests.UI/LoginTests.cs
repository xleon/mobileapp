using NUnit.Framework;
using Xamarin.UITest.iOS;
using static Toggl.Daneel.Tests.UI.Extensions.LoginExtensions;

namespace Toggl.Daneel.Tests.UI
{
    [TestFixture]
    public class LoginTests
    {
        private const string validEmail = "susancalvin@psychohistorian.museum";

        private iOSApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = Configuration.GetApp();

            app.WaitForLoginScreen();
        }

        [Test]
        public void TheNextButtonShowsThePasswordField()
        {
            app.EnterText(validEmail);

            app.GoToPasswordScreen();

            app.Screenshot("Login password page.");
        }

        [Test]
        public void TheBackButtonClosesTheLoginViewIfTheEmailFieldIsVisible()
        {
            app.GoBackToOnboardingScreen();

            app.Screenshot("Onboarding last page.");
        }

        [Test]
        public void TheBackButtonShowsTheEmailFieldIfThePasswordFieldIsVisible()
        {
            app.EnterText(validEmail);
            app.GoToPasswordScreen();

            app.GoBackToEmailScreen();

            app.Screenshot("Login email page.");
        }

        [Test]
        public void TheNextButtonAfterInputtingAnInvalidPasswordShowsTheErrorLabel()
        {
            app.EnterText(Credentials.Username);
            app.GoToPasswordScreen();

            app.EnterText($"{Credentials.Password}123456");
            app.TryLoginAndFail();

            app.Screenshot("Login email page.");
        }

        [Test]
        public void TheNextButtonAfterInputtingAValidPasswordShowsTheMainScreen()
        {
            app.EnterText(Credentials.Username);
            app.GoToPasswordScreen();

            app.EnterText(Credentials.Password);
            app.LoginSuccesfully();

            app.Screenshot("Login email page.");
        }
    }
}
