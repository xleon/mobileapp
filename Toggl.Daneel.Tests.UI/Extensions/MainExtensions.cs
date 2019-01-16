using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class MainExtensions
    {
        public static void TapNthCellInCollection(this IApp app, int index)
        {
            app.Tap(query => query.Class("UITableViewCell").Child(index));
        }
    }
}