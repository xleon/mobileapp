using Toggl.Core.Models;
using Toggl.Core.UI.Interfaces;
using Toggl.Core.UI.Models;

namespace Toggl.Core.UI.ViewModels.DateRangePicker
{
    public sealed partial class DateRangePickerViewModel
    {
        public struct Shortcut : IDiffableByIdentifier<Shortcut>
        {
            public static Shortcut From(DateRangeShortcut shortcut, bool isSelected)
                => new Shortcut(shortcut.Period, shortcut.Text, isSelected);

            private Shortcut(DateRangePeriod period, string text, bool isSelected)
            {
                DateRangePeriod = period;
                Text = text;
                IsSelected = isSelected;
            }

            public string Text { get; }
            public DateRangePeriod DateRangePeriod { get; }
            public bool IsSelected { get; }

            public long Identifier
                => (long)DateRangePeriod;

            public bool Equals(Shortcut other)
                => other.DateRangePeriod == DateRangePeriod
                && other.IsSelected == IsSelected;
        }
    }
}
