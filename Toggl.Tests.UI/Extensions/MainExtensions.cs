using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class MainExtensions
    {
        public static void WaitForMainScreen(this IApp app)
        {
            var email = $"{Guid.NewGuid().ToString()}@toggl.space";

            app.WaitForSignUpScreen();

            app.Tap(SignUp.EmailText);
            app.EnterText(email);
            app.Tap(SignUp.PasswordText);
            app.EnterText("123456");
            app.SignUpSuccesfully();
        }

        public static void StartTimeEntryWithDescription(this IApp app, string description)
        {
            app.Tap(Main.StartTimeEntryButton);
            app.WaitForElement(StartTimeEntry.DoneButton);

            app.EnterText(description);
            app.Tap(StartTimeEntry.DoneButton);

            app.WaitForElement(Main.StopTimeEntryButton);
        }

        public static void TapOnTimeEntryWithDescription(this IApp app, string description)
        {
            var timeEntryCellSelector = queryForTimeEntryCell(description);
            app.WaitForElement(timeEntryCellSelector);
            app.Tap(timeEntryCellSelector);
        }

        public static void SwipeEntryToDelete(this IApp app, string timeEntryDescription)
        {
            var timeEntryCellRect = RectForTimeEntryCell(app, timeEntryDescription);

            app.DragCoordinates(
                fromX: timeEntryCellRect.X + timeEntryCellRect.Width,
                fromY: timeEntryCellRect.CenterY,
                toX: timeEntryCellRect.X,
                toY: timeEntryCellRect.CenterY
            );

            app.WaitForNoElement(x => x.Text(timeEntryDescription));
        }

        public static void PullToRefresh(this IApp app)
        {
            app.WaitForNoElement(query => query.Text("Synced"));
            app.ScrollUp(Main.TimeEntriesCollection, ScrollStrategy.Gesture);
            app.WaitForNoElement(query => query.Text("Synced"));
        }

        public static void StopTimeEntry(this IApp app)
        {
            app.WaitForElement(Main.StopTimeEntryButton);
            app.Tap(Main.StopTimeEntryButton);
            app.WaitForNoElement(Main.StopTimeEntryButton);
        }

        public static AppRect RectForTimeEntryCell(this IApp app, string timeEntryDescription)
        {
            var timeEntryViews = app.Query(queryForTimeEntryCell(timeEntryDescription));
            if (timeEntryViews.Length == 0)
                return null;
            return timeEntryViews[0].Rect;
        }

        private static Func<AppQuery, AppQuery> queryForTimeEntryCell(string timeEntryDescription)
        => x => x.Marked(Main.TimeEntryRow).Descendant().Text(timeEntryDescription);
    }
}
