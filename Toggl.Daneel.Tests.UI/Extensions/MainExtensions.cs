using Xamarin.UITest;

namespace Toggl.Daneel.Tests.UI.Extensions
{
    public static class MainExtensions
    {
        public static void WaitForMainScreen(this IApp app)
        {
            app.WaitForLoginScreen();

            app.EnterText(Credentials.Username);
            app.GoToPasswordScreen();

            app.EnterText(Credentials.Password);
            app.LoginSuccesfully();
        }

        public static void StartTimeEntryWithDescription(this IApp app, string description)
        {
            app.Tap(Main.StartTimeEntryButton);
            app.WaitForElement(StartTimeEntry.DoneButton);

            app.EnterText(description);
            app.Tap(StartTimeEntry.DoneButton);

            app.WaitForElement(Main.StopTimeEntryButton);
        }
    }
}
