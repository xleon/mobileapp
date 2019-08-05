using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Toggl.Tests.UI.Helpers;
using Xamarin.UITest;
using static Toggl.Tests.UI.Extensions.LoginExtensions;

namespace Toggl.Tests.UI
{
    [TestFixture]
    public sealed class LoginTests
    {
        private const string validEmail = "susancalvin@psychohistorian.museum";

        private IApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = Configuration.GetApp();
        }

        [Test]
        public void TheLoginButtonAfterInputtingAnInvalidCredentialsShowsTheErrorLabel()
        {
            var email = randomEmail();
            var password = "asdads";

            app.Tap(Login.EmailText);
            app.EnterText(email);
            app.Tap(Login.PasswordText);
            app.EnterText($"{password}123456");
            app.Tap(Login.LoginButton);

            app.AssertLoginFailed();
        }

        [Test, IgnoreOnIos]
        public void TheLoginButtonIsDisabledWhenEnteringMalformattedEmail()
        {
            var email = "asdasd@asdasd";
            var password = "asdads";

            app.Tap(Login.EmailText);
            app.EnterText(email);
            app.Tap(Login.PasswordText);
            app.EnterText($"{password}123456");

            app.AssertLoginButtonDisabled();
        }

        [Test]
        public async Task TheLoginButtonAfterInputtingValidCredentialsShowTheMainScreen()
        {
            var email = randomEmail();
            var password = await User.Create(email);

            app.Tap(Login.EmailText);
            app.EnterText(email);
            app.Tap(Login.PasswordText);
            app.EnterText(password);
            app.Tap(Login.LoginButton);

            app.AssertLoggedInSuccesfully();
        }

        private string randomEmail()
            => $"{Guid.NewGuid().ToString()}@toggl.space";
    }
}
