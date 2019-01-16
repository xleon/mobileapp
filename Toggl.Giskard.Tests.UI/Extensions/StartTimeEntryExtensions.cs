using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class StartTimeEntryExtensions
    {
        public static void TapCreateTag(this IApp app, string tagName)
        {
            app.Tap($"Create tag \"{tagName}\"");
        }

        public static void TapCreateProject(this IApp app, string projectName)
        {
            app.Tap($"Create project \"{projectName}\"");
        }
    }
}