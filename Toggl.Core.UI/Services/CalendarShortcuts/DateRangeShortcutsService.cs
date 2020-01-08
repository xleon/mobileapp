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
    public partial class DateRangeShortcutsService : IDateRangeShortcutsService
    {
        private readonly ITimeService timeService;
        private IDisposable userDisposable;

        public ImmutableList<DateRangeShortcut> Shortcuts { get; private set; }

        public DateRangeShortcut GetShortcutFrom(DateRange range)
            => Shortcuts.FirstOrDefault(s => s.MatchesDateRange(range));

        public DateRangeShortcut GetShortcutFrom(DateRangePeriod period)
            => Shortcuts.FirstOrDefault(s => s.Period == period);

        public DateRangeShortcutsService(ITogglDataSource dataSource, ITimeService timeService)
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

        private IEnumerable<DateRangeShortcut> createShortcuts(BeginningOfWeek beginningOfWeek)
        {
            var today = timeService.CurrentDateTime.ToLocalTime().Date;

            yield return new TodayDateRangeShortcut(today);
            yield return new YesterdayDateRangeShortcut(today);
            yield return new ThisWeekDateRangeShortcut(today, beginningOfWeek);
            yield return new LastWeekDateRangeShortcut(today, beginningOfWeek);
            yield return new ThisMonthDateRangeShortcut(today);
            yield return new LastMonthDateRangeShortcut(today);
            yield return new ThisYearDateRangeShortcut(today);
            yield return new LastYearDateRangeShortcut(today);
        }
    }
}
