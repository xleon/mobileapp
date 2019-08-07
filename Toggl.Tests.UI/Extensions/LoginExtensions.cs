using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class LoginExtensions
    {
        public static void AssertLoginFailed(this IApp app)
        {
            app.WaitForElement(Login.ErrorLabel);
        }

        public static void AssertLoggedInSuccesfully(this IApp app)
        {
            app.WaitForElement(Main.StartTimeEntryButton);
        }
    }
}
