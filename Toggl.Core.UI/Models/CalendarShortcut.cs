using System;
using Toggl.Core.Models;
using Toggl.Shared;

namespace Toggl.Core.UI.Models
{
    public abstract class CalendarShortcut
    {
        public virtual string Text { get; protected set; }
        public virtual ReportPeriod Period { get; protected set; }
        public virtual DateRange DateRange { get; protected set; }

        public virtual bool MatchesDateRange(DateRange range)
            => range == DateRange;
    }
}
