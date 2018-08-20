using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels.Selectable
{
    public sealed class SelectableUserCalendarViewModel
    {
        public string Id { get; }

        public string Name { get; }

        public string SourceName { get; }

        public bool Selected { get; }

        private SelectableUserCalendarViewModel(SelectableUserCalendarViewModel vm, bool selected)
        {
            Id = vm.Id;
            Name = vm.Name;
            SourceName = vm.SourceName;
            Selected = selected;
        }

        public SelectableUserCalendarViewModel(UserCalendar calendar, bool selected)
        {
            Ensure.Argument.IsNotNull(calendar, nameof(calendar));

            Id = calendar.Id;
            Name = calendar.Name;
            SourceName = calendar.SourceName;
            Selected = selected;
        }

        public SelectableUserCalendarViewModel InvertSelected()
            => new SelectableUserCalendarViewModel(this, !Selected);
    }
}
