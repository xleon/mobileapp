using Toggl.Core.UI.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.Selectable
{
    [Preserve]
    public sealed class SelectableDateFormatViewModel : IDiffableByIdentifier<SelectableDateFormatViewModel>
    {
        public long Identifier => DateFormat.GetHashCode(); 

        public DateFormat DateFormat { get; }

        public bool Selected { get; set; }

        public SelectableDateFormatViewModel(DateFormat dateFormat, bool selected)
        {
            DateFormat = dateFormat;
            Selected = selected;
        }

        public bool Equals(SelectableDateFormatViewModel other)
            => DateFormat == other.DateFormat && Selected == other.Selected;
    }
}
