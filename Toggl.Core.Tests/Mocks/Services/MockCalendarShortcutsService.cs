using NSubstitute;
using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Models;
using Toggl.Core.UI.Models;
using Toggl.Core.UI.Services;
using Toggl.Shared;

namespace Toggl.Core.Tests.Mocks.Services
{
    public sealed class MockCalendarShortcutsService : ICalendarShortcutsService
    {
        private ICalendarShortcutsService wrappedService;

        public MockCalendarShortcutsService()
        {
            var dataSource = Substitute.For<ITogglDataSource>();
            var timeService = Substitute.For<ITimeService>();

            var user = new MockUser { Id = 1, BeginningOfWeek = BeginningOfWeek.Wednesday };
            dataSource.User.Current.Returns(Observable.Return(user));

            var currentTime = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);
            timeService.CurrentDateTime.Returns(currentTime);

            wrappedService = new CalendarShortcutsService(dataSource, timeService);
        }

        public CalendarShortcut GetShortcutFrom(ReportPeriod period)
            => wrappedService.GetShortcutFrom(period);

        public CalendarShortcut GetShortcutFrom(DateRange range)
            => wrappedService.GetShortcutFrom(range);

        public ImmutableList<CalendarShortcut> Shortcuts
            => wrappedService.Shortcuts;

        public void Dispose()
            => wrappedService.Dispose();
    }
}
