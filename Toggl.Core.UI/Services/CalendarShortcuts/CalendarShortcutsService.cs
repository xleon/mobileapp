using System;
using System.Reactive.Linq;
using System.Collections.Immutable;
using Toggl.Core.DataSources;
using Toggl.Shared;
using Toggl.Core.UI.Models;
using System.Collections.Generic;
using Toggl.Core.Models;
using System.Linq;

namespace Toggl.Core.UI.Services
{
    public partial class CalendarShortcutsService : ICalendarShortcutsService
    {
        private readonly ITimeService timeService;
        private IDisposable userDisposable;

        public ImmutableList<CalendarShortcut> Shortcuts { get; private set; }

        public CalendarShortcut GetShortcutFrom(DateRange range)
            => Shortcuts.FirstOrDefault(s => s.MatchesDateRange(range));

        public CalendarShortcut GetShortcutFrom(ReportPeriod period)
            => Shortcuts.FirstOrDefault(s => s.Period == period);

        public CalendarShortcutsService(ITogglDataSource dataSource, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;

            userDisposable =
                dataSource.User.Current
                .Subscribe(user => updateShortcuts(user.BeginningOfWeek));
        }

        private void updateShortcuts(BeginningOfWeek beginningOfWeek)
        {
            Shortcuts = createShortcuts(beginningOfWeek).ToImmutableList();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
                return;

            userDisposable?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private IEnumerable<CalendarShortcut> createShortcuts(BeginningOfWeek beginningOfWeek)
        {
            var today = timeService.CurrentDateTime.ToLocalTime().Date;

            yield return new TodayCalendarShortcut(today);
            yield return new YesterdayCalendarShortcut(today);
            yield return new ThisWeekCalendarShortcut(today, beginningOfWeek);
            yield return new LastWeekCalendarShortcut(today, beginningOfWeek);
            yield return new ThisMonthCalendarShortcut(today);
            yield return new LastMonthCalendarShortcut(today);
            yield return new ThisYearCalendarShortcut(today);
            yield return new LastYearCalendarShortcut(today);
        }
    }
}
