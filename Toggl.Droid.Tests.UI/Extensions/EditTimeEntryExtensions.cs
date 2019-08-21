using System.Linq;
using Xamarin.UITest;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class EditTimeEntryExtensions
    {
        public static void ConfirmEditTimeEntry(this IApp app)
        {
            app.Tap(EditTimeEntry.Confirm);

            //On Android the confirm button sometimes need to be tapped twice.
            //Once to finish editing the description and once more to close the edit view
            if (app.Query(EditTimeEntry.Confirm).Length > 0)
                app.Tap(EditTimeEntry.Confirm);

            app.WaitForNoElement(EditTimeEntry.Confirm);
        }

        public static bool TagsAreCorrect(this IApp app, string[] tags)
        {
            var foundTags = app.Query(x => x.Marked(EditTimeEntry.TagsList).Child().Child()).Select(x => x.Text);
            return tags.SequenceEqual(foundTags);
        }

        public static bool BillableStateIsCorrect(this IApp app, bool isBillable)
            => app.Query(x => x.Marked(EditTimeEntry.Billable)).Any()
                && app.Query(x => x.Marked(EditTimeEntry.Billable).Invoke("isChecked").Value<bool>()).First() == isBillable;
    }
}
