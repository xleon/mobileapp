using Toggl.Core.Models;
using Toggl.Core.UI.Interfaces;
using Toggl.Core.UI.Models;

namespace Toggl.Core.UI.ViewModels.DateRangePicker
{
    public sealed partial class DateRangePickerViewModel
    {
        public struct Shortcut : IDiffableByIdentifier<Shortcut>
        {
            public static Shortcut From(CalendarShortcut shortcut, bool isSelected)
                => new Shortcut(shortcut.Period, shortcut.Text, isSelected);

            private Shortcut(ReportPeriod period, string text, bool isSelected)
            {
                ReportPeriod = period;
                Text = text;
                IsSelected = isSelected;
            }

            public string Text { get; }
            public ReportPeriod ReportPeriod { get; }
            public bool IsSelected { get; }

            public long Identifier
                => (long)ReportPeriod;

            public bool Equals(Shortcut other)
                => other.ReportPeriod == ReportPeriod
                && other.IsSelected == IsSelected;
        }
    }
}
