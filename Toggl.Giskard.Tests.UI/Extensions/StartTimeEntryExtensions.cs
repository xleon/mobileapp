using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class StartTimeEntryExtensions
    {
        public static void TapCreateTag(this IApp app, string tagName)
        {
            var query = $"Create tag \"{tagName}\"";
            tapAndWaitForCreateElement(app, query);
        }

        public static void TapCreateProject(this IApp app, string projectName)
        {
            var query = $"Create project \"{projectName}\"";
            tapAndWaitForCreateElement(app, query);
        }

        private static void tapAndWaitForCreateElement(IApp app, string query)
        {
            app.WaitForElement(query);
            app.Tap(query);
            app.WaitForNoElement(query);
        }
    }
}
