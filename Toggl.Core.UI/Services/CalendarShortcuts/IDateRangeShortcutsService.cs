using System;
using System.Collections.Immutable;
using Toggl.Core.Models;
using Toggl.Core.UI.Models;
using Toggl.Shared;

namespace Toggl.Core.UI.Services
{
    public interface IDateRangeShortcutsService : IDisposable
    {
        ImmutableList<DateRangeShortcut> Shortcuts { get; }

        DateRangeShortcut GetShortcutFrom(DateRangePeriod period);
        DateRangeShortcut GetShortcutFrom(DateRange range);
    }
}
