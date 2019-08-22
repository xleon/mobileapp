using System.Linq;
using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class EditTimeEntryExtensions
    {
        public static void ConfirmEditTimeEntry(this IApp app)
        {
            app.Tap(EditTimeEntry.Confirm);

            app.WaitForNoElement(EditTimeEntry.Confirm);
        }

        public static bool TagsAreCorrect(this IApp app, string[] tags)
        {
            var label = app.Query(x => x.Marked(EditTimeEntry.EditTags))[0].Label;

            if (tags.Length == 0
                && label != "No tags")
                return false;

            if (!label.Contains(string.Join(", ", tags)))
                return false;

            return true;
        }

        public static bool BillableStateIsCorrect(this IApp app, bool isBillable)
            => app.Query(x => x.Marked(EditTimeEntry.Billable).Text(isBillable ? "1" : "0")).Any();
    }
}
