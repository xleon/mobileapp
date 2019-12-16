using System;
using System.Collections.Immutable;
using Toggl.Core.Models;
using Toggl.Core.UI.Models;
using Toggl.Shared;

namespace Toggl.Core.UI.Services
{
    public interface ICalendarShortcutsService : IDisposable
    {
        ImmutableList<CalendarShortcut> Shortcuts { get; }

        CalendarShortcut GetShortcutFrom(ReportPeriod period);
        CalendarShortcut GetShortcutFrom(DateRange range);
    }
}
